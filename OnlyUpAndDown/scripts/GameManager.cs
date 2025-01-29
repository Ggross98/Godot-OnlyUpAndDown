using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public partial class GameManager : Node2D
{
	[Export] private Grid grid;
	[Export] private TileManager tileManager;
	[Export] private CharacterManager characterManager;
	[Export] private ItemManager itemManager;
	[Export] private UIManager uiManager;
	// [Export] private SkillBarManager skillBarManager;


	// private Character player;
	private PlayerStatus playerStatus;
	private GameStatus gameStatus;

	private LevelData levelData;

	/// <summary>
	/// 当前游戏状态。Waiting: 游戏中，等候玩家操作; Animating: 游戏中，播放动画，无法操作; Initializing: 初始化中; Over: 结束状态;
	/// </summary>
	public enum SceneState { Waiting, Animating, Initializing, Paused, Over };
	private SceneState state = SceneState.Initializing;
	
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private Tween tween;

	private StaticData staticData;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// characterManager = GetNode<CharacterManager>("/root/Game/Board/CharacterManager");
		// tileManager = GetNode<TileManager>("/root/Game/Board/TileManager");
		staticData = GetNode<StaticData>("/root/StaticData");

		gameStatus = new GameStatus();

		playerStatus = PlayerStatus.Copy(staticData.startPlayerStatus);
		uiManager.ShowPlayerStatus(playerStatus);

		var ld = GenerateLevelData();
		LoadLevel(ld);

	}

	public void QuitGame(){
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}

	private void ChangePlayerStatus(string status, int delta, ValueSource source){

		if(delta == 0) return;

		if(HasArtifact(27) && (delta > 0 && status == "HP")){
			ChangePlayerStatus("mana", 1, ValueSource.Artifact);
			return;
		}

		if(status == "HP" || status == "shield" && source == ValueSource.Skill && delta > 0){
			delta += playerStatus.mana;
		}

		var gPos = characterManager.Player.GlobalPosition;
		string iconName = "";
		switch(status){
			case "HP":
				iconName = "heart";
				playerStatus.ChangeHP(delta);
				break;
			case "shield":
				iconName = "shield";
				playerStatus.ChangeShield(delta);
				break;
			case "money":
				iconName = "coin";
				playerStatus.ChangeMoney(delta);
				break;
			case "mana":
				iconName = "mana";
				playerStatus.ChangeMana(delta);
				break;
		}

		if(iconName != ""){
			uiManager.ShowJumpingLabel(gPos, iconName, delta);
		}

		uiManager.ShowPlayerStatus(playerStatus);
	}

	public enum ValueSource {
		Skill, Artifact, Battle, Food, Trap, Buy, Resource
	};

	private void DamageEnemy(Character enemy, int damage, ValueSource source){
		if(enemy == null) return;

		// 魔力增伤
		if(source == ValueSource.Skill) damage += playerStatus.mana;

		// 上海人偶
		if(HasArtifact(22) && source == ValueSource.Artifact) damage += 1;

		// 妖精契约
		if(HasArtifact(8) && enemy.Data.race == CharacterData.Race.Yousei) damage *= 2;

		if(damage <= 0) return;

		enemy.ChangeHP(-damage);
		enemy.ShowAnimation("hit");
		uiManager.ShowJumpingLabel(enemy.GlobalPosition, "heart", -damage);
	}

	private void DamagePlayer(int damage, ValueSource source, Character enemy = null){
		
		// 菜刀
		if(HasArtifact(7) && source == ValueSource.Battle && enemy.Data.race == CharacterData.Race.Human) damage /= 2;

		// 扫地机器人
		if(HasArtifact(19) && source == ValueSource.Trap) damage /= 2;

		// 鸦天狗羽毛
		if(HasArtifact(23) && playerStatus.shield <= 0) damage = (int)(damage * 0.8f);
		
		if(damage <= 0) return;

		if(damage <= playerStatus.shield) {
			ChangePlayerStatus("shield", -damage, source);
			damage = 0;
		}
		else{

			// 灯笼裤
			if(HasArtifact(24)){
				if(playerStatus.shield > 0){
					damage -= playerStatus.shield;
					if(damage > 15) damage = 15;
				}
			}
			else{
				damage -= playerStatus.shield;
			}
			
			playerStatus.shield = 0;
		}

		if(damage <= 0) return;

		characterManager.Player.ShowAnimation("hit");
		ChangePlayerStatus("HP", -damage, source);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if(state == SceneState.Waiting){

			if(Input.IsActionJustPressed("pause")){
				Pause();
				return;
			}

			// Scan input
			var player = characterManager.Player;
			if(player != null){

				Vector2I dir = Vector2I.Zero;

				if(Input.IsActionJustPressed("to_up")) dir = new Vector2I(0, 1);
				else if(Input.IsActionJustPressed("to_down")) dir = new Vector2I(1, 0);

				if(dir != Vector2I.Zero){

					var target = player.Coord + dir;
					if(grid.IsInRange(target)){
						
						state = SceneState.Animating;

						if(tween != null) tween.Kill();
						tween = CreateTween();

						// Use skill
						uiManager.EnterDirection(Utils.Vector2Direction(dir));
						var skills = uiManager.GetReadySkills();
						foreach(var skill in skills){
							UseSkill(skill, dir);
						}
						tween.TweenCallback(Callable.From(()=>{
							UpdateTiles();
						}));

						// Battle
						var enemy = characterManager.GetAt(target);
						if(enemy != null && enemy != player){
							tween.TweenCallback(Callable.From(()=>{ 
								ProcessArtifactsBeforeBattle(enemy);
							}));

							ProcessBattle(enemy);

							tween.TweenCallback(Callable.From(()=>{ 
								ProcessArtifactsAfterBattle();
							}));

							UpdateTiles();
						}

						// Move
						MovePlayer(target);
						tween.TweenCallback(Callable.From(()=>{ 
							ProcessArtifactsAfterMove(dir);
						}));
						

						// Process event at target position
						ProcessAt(target);
						
						// 动画完成后检查玩家和游戏状态
						tween.TweenCallback(Callable.From(()=>{ 
							uiManager.ShowPlayerStatus(playerStatus);
							// Check level state
							if(!playerStatus.IsAlive()){
								GameLose();
							}else{
								if(target == grid.endPoint){
									LevelClear();
								}
							}

							// Reset scene state
							if(state == SceneState.Animating){
								state = SceneState.Waiting; 
							}
						}));
					}
				}

			}
		}
		
	}

	/// <summary>
	/// 生成当前关卡的地图数据
	/// </summary>
	/// <returns></returns>
	private LevelData GenerateLevelData(){

		// 每5层为一个循环，前4层为战斗层，第五层为奖励层。
		
		var ld = new LevelData();
		ld.type = gameStatus.currentLevel % 5 == 4 ? LevelData.LevelType.Bonus : LevelData.LevelType.Normal;

		// 每5层更换一个地形
		if(gameStatus.currentLevel % 5 == 0) 
			gameStatus.currentTerrain = rng.RandiRange(0, tileManager.MapTerrains());
		ld.mapTerrain = gameStatus.currentTerrain;

		switch(ld.type){
			case LevelData.LevelType.Normal:

				// 战斗层长度为4、5、6、7
				ld.mapWidth = 4 + gameStatus.currentLevel % 5 * 1;
				ld.mapHeight = 3;
				
				// 非怪物格子中出现遗物的概率
				ld.pArtifact = 0.2f;

				// 出现怪物的概率
				ld.pEnemy = 0.4f;

				break;

			case LevelData.LevelType.Bonus:

				// 奖励层的尺寸固定为3*3
				ld.mapWidth = 3;
				ld.mapHeight = 3;

				// 奖励层出现遗物、怪物为固定，不设置

				break;
		}

		ld.enemyDataList = new List<CharacterData>(){};
		for(int i = 0; i < 6; i++){
			ld.enemyDataList.Add(staticData.GetCharacterData(i));
		}

		return ld;
	}

	/// <summary>
	/// 显示指定格子上的道具和状态
	/// </summary>
	/// <param name="pos"></param>
	public void UpdateTileAt(Vector2I pos){
		var tile = tileManager.GetAt(pos);

		var enemy = characterManager.GetAt(pos); 
		if(enemy != null && enemy.isEnemy){
			tile.ShowEnemy(enemy);
		}
		else{
			var item = itemManager.GetAt(pos);
			if(item != null){
				tile.ShowItem(item);
			}
			else{
				tile.ClearInfo();
			}
		}

		
	}

	/// <summary>
	/// 显示所有格子上的道具和状态
	/// </summary>
	public void UpdateTiles(){
		foreach(var pos in grid.range){
			UpdateTileAt(pos);
		}
	}

	public void ChangeTilesColor(){
		foreach(var pos in grid.range){
			var tile = tileManager.GetAt(pos);
			tile.SetModule(grid.IsReachable(characterManager.Player.Coord, pos) ? new Color(1, 1, 1) : new Color(0.2f, 0.2f, 0.2f, 0.5f));
		}
	}

	/// <summary>
	/// 使用指定技能
	/// </summary>
	/// <param name="skillData"></param>
	public void UseSkill(SkillData skillData, Vector2I dir){

		uiManager.ResetSkill(skillData);

		var nextEnemy = characterManager.GetAt(characterManager.Player.Coord + dir);
		var enemies = characterManager.GetReachableEnemies();
		var player = characterManager.Player;

		switch(skillData.id){
			case 0: // 灵击
				tween.TweenCallback(Callable.From(()=>{
					DamageEnemy(nextEnemy, 4, ValueSource.Skill);
				}));
				break;
			case 1: // 魔法药水
				tween.TweenCallback(Callable.From(()=>{
					ChangePlayerStatus("HP", 5, ValueSource.Skill );
				}));
				break;
			case 2: // 替身人偶
				tween.TweenCallback(Callable.From(()=>{
					ChangePlayerStatus("shield", 5, ValueSource.Skill);
				}));
				break;
			case 3: // 铜钱
				tween.TweenCallback(Callable.From(()=>{
					ChangePlayerStatus("money", 2, ValueSource.Skill);
				}));
				break;
			case 4: // 梦想妙珠
				tween.TweenCallback(Callable.From(()=>{
					var idx = rng.RandiRange(0, enemies.Count - 1);
					var enemy = enemies[idx];
					DamageEnemy(enemy, 10, ValueSource.Skill);
				}));
				break;
			case 5: // 封魔阵
				tween.TweenCallback(Callable.From(()=>{
					DamageEnemy(nextEnemy, 8, ValueSource.Skill);
				}));
				break;
			case 6: // 梦想天生;
				tween.TweenCallback(Callable.From(()=>{
					foreach (var enemy in Utils.GetRandomChildren(enemies, 3, rng)){
						DamageEnemy(enemy, 999, ValueSource.Skill);
					}
				}));
				break;
			case 7: // 魔法废弃炸弹
				tween.TweenCallback(Callable.From(()=>{
					foreach (var enemy in Utils.GetRandomChildren(enemies, 1, rng)){
						DamageEnemy(enemy, 999, ValueSource.Skill);
					}
				}));
				break;
			case 8: // 极限火花
				tween.TweenCallback(Callable.From(()=>{
					foreach(var enemy in enemies){
						if((enemy.Coord.X == player.Coord.X && dir.Equals(new Vector2I(0, 1))) 
							|| (enemy.Coord.Y == player.Coord.Y && dir.Equals(new Vector2I(01, 0)))){
							DamageEnemy(enemy, 6, ValueSource.Skill);
						}
						}}
					));
				
				break;
			case 9: // 先忧后乐之剑
				tween.TweenCallback(Callable.From(()=>{
					if(enemies.Count > 0){
						var damage = (int)Math.Ceiling(20.0 / enemies.Count);
						foreach(var enemy in enemies){
							DamageEnemy(enemy, damage, ValueSource.Skill);
						}
					}
				}));
				
				break;
			case 10: // 热核护罩
				tween.TweenCallback(Callable.From(()=>{
					DamageEnemy(nextEnemy, playerStatus.shield, ValueSource.Skill);
				}));
				
				break;
			case 11: // 无寿之梦
				tween.TweenCallback(Callable.From(()=>{
					foreach(var enemy in enemies){
						DamageEnemy(enemy, playerStatus.maxHP - playerStatus.HP, ValueSource.Skill);
					}
				}));
				
				break;
			case 12: // 咲夜的世界
				// foreach(var skillBar in uiManager.sk)
				break;
			case 13: // 八重雾之渡
				tween.TweenCallback(Callable.From(()=>{
					foreach(var enemy in enemies){
						DamageEnemy(enemy, playerStatus.money, ValueSource.Skill);
						ChangePlayerStatus("money", -1, ValueSource.Skill);
					}
				}));
				break;
			case 14: // 国士无双之药
				tween.TweenCallback(Callable.From(()=>{
					ChangePlayerStatus("HP", 5, ValueSource.Skill);
					ChangePlayerStatus("shield", 5, ValueSource.Skill);
				}));
				break;
			case 15: // 红色不夜城
				tween.TweenCallback(Callable.From(()=>{
					if(playerStatus.HP >= 5){
						var delta = playerStatus.HP / 2;
						ChangePlayerStatus("HP", -delta, ValueSource.Skill);
						ChangePlayerStatus("shield", delta, ValueSource.Skill);
					}
					
				}));
				break;
			case 16: // 皇家烈焰
				tween.TweenCallback(Callable.From(()=>{
					foreach(var enemy in enemies){
						DamageEnemy(enemy, 4, ValueSource.Skill);
					}
				}));
				break;
		}
		tween.Parallel().TweenCallback(Callable.From(()=>{
			player.ShowAnimation("attack");
		}));
		tween.TweenCallback(Callable.From(()=>{ 
			ProcessArtifactsAfterSkill();
		 })).SetDelay(Settings.ANIMATION_INTERVAL);
		
	}

	/// <summary>
	/// 移动玩家
	/// </summary>
	/// <param name="target"></param>
	public void MovePlayer(Vector2I target){
		
		characterManager.MovePlayer(target, tween);
		tween.TweenCallback(Callable.From(()=>{
			ChangeTilesColor();
		}));
		// UpdateTiles();
	}

	public void Pause(){
		if(state == SceneState.Waiting){
			state = SceneState.Paused;
			uiManager.ShowPausePanel();
		}
		
	}

	public void Resume(){
		if(state == SceneState.Paused){
			state = SceneState.Waiting;
			uiManager.HidePausePanel();
		}
	}

	public void GameWin(){
		GD.Print("胜利!");
		state = SceneState.Over;

		uiManager.ShowOverPanel(true);
	}

	public void GameLose(){
		GD.Print("游戏结束");
		state = SceneState.Over;

		uiManager.ShowOverPanel(false);
	}

	/// <summary>
	/// 处理通过一层后的事件
	/// </summary>
	public void LevelClear(){
		GD.Print("Level " + gameStatus.currentLevel + " clear!");

		// Clear current objects
		tileManager.ClearObjects();
		characterManager.ClearObjects();
		itemManager.ClearObjects();

		if(gameStatus.currentLevel >= Settings.MAX_DEPTH * 5 + 4){
			GameWin();
		}else{
			// Create and load new level data
			gameStatus.currentLevel += 1;
			LoadLevel(GenerateLevelData());
		}

		
	}

	/// <summary>
	/// 处理指定位置格子上的全部事件
	/// </summary>
	/// <param name="coord"></param>
	public void ProcessAt(Vector2I coord){

		var player = characterManager.Player;

		// Item
		var item = itemManager.GetAt(coord);
		if(item != null){
			tween.TweenCallback(Callable.From(()=>{
				ProcessItem(item);
				UpdateTiles();
			}));
			
		}

		// Enemy
		var enemy = characterManager.GetEnemyAt(coord);
		if(enemy != null){

			tween.TweenCallback(Callable.From(()=>{
				int coin = 1;

				// 妖魔书
				if(HasArtifact(6) && enemy.Data.race == CharacterData.Race.Yokai) coin += 2;
				
				ChangePlayerStatus("money", coin, ValueSource.Skill);
				characterManager.DeleteObject(enemy);

				UpdateTiles();
				// tileManager.GetAt(coord).ClearInfo();
				// uiManager.ShowPlayerStatus(playerStatus);
			}));
			


		}

		// tween.TweenCallback(Callable.From(()=>{ 
		// 	// tileManager.GetAt(coord).ClearInfo();
		// 	// uiManager.ShowPlayerStatus(playerStatus);
		//  })).SetDelay(0.5f);
		
	}

	public void ProcessBattle(Character enemy){
		tween.TweenCallback(Callable.From(()=>{
			var damage = enemy.HP;
			if(damage > 0){
				enemy.ShowAnimation("attack");
				DamagePlayer(damage, ValueSource.Battle, enemy);
			}
			// DamagePlayer(damage, DamageSource.Battle, enemy);
			// if(damage > 0){
				
			// 	enemy.ShowAnimation("attack");
			// 	characterManager.Player.ShowAnimation("hit");

			// 	if(playerStatus.shield >= damage){
			// 		playerStatus.shield -= damage;
			// 	}else{
					
			// 		if(playerStatus.shield == 0 && HasArtifact(23)){
			// 			damage = 0;
			// 		}
			// 		else{
			// 			damage -= playerStatus.shield;
			// 			ChangePlayerStatus("shield", playerStatus.shield);

			// 			if(!HasArtifact(24)){
			// 				ChangePlayerStatus("HP", -damage);
			// 			}
			// 		}

					
					
			// 	}
			// }

			
		}));
		// tween.TweenCallback(Callable.From(()=>{
		// 	characterManager.Player.ShowAnimation("attack");
		// 	DamageEnemy(enemy, enemy.HP);
		// })).SetDelay(0.5f);
		
		tween.TweenCallback(Callable.From(()=>{  })).SetDelay(Settings.ANIMATION_INTERVAL);
		

	}


	private void ProcessArtifactsLoadLevel(){
		var enemies = characterManager.GetEnemies();
		foreach(var a in playerStatus.artifacts){

			switch(a.id){
				case 21: // 天狗盾
					if(playerStatus.shield == 0){
						ChangePlayerStatus("shield", 20, ValueSource.Artifact);
					}
					break;
				case 0: // 红色蝴蝶结
					ChangePlayerStatus("shield", 10, ValueSource.Artifact);
					break;
				case 3: // 魔女帽
					foreach(var enemy in enemies){
						DamageEnemy(enemy, 2, ValueSource.Artifact);
					}
					break;
				case 10: //魔法炸弹
					var tmp0 = enemies[rng.RandiRange(0, enemies.Count - 1)];
					DamageEnemy(tmp0, 999, ValueSource.Artifact);
					break;
				case 15: // 守矢护符
					ChangePlayerStatus("HP", 10, ValueSource.Artifact);
					break;
				case 20: // 四叶草
					var items = itemManager.GetReachableItems(new Vector2I(0, 0));
					if(items.Count > 0){
						var idx = rng.RandiRange(0, items.Count);
						items[idx].value *= 3;
					}
					break;
				case 25: // 天狗面具
					if(gameStatus.currentLevel % 5 == 0){
						ChangePlayerStatus("mana", 2, ValueSource.Artifact);
					}
					break;
			}

		}
	}

	private void ProcessArtifactsAfterSkill(){
		foreach(var a in playerStatus.artifacts){
			switch(a.id){
				case 13: // 龙鱼羽衣
					ChangePlayerStatus("HP", 3, ValueSource.Artifact);
					break;
				case 5: // 八卦炉
					ChangePlayerStatus("mana", 1, ValueSource.Artifact);
					break;
			}
		}
	}

	private void ProcessArtifactsAfterMove(Vector2I dir){
		foreach(var a in playerStatus.artifacts){
			switch (a.id){
				case 4: // 扫帚
					ChangePlayerStatus("shield", 1, ValueSource.Artifact);
					break;
				case 9: // 烤红薯
					var foods = itemManager.GetReachableItems(characterManager.Player.Coord, ItemData.Type.Food);
					foreach(var food in foods){
						food.value += 1;
					}
					break;
				case 11: // 火箭模型
					if(dir.Equals(new Vector2I(0, 1))){
						ChangePlayerStatus("shield", 2, ValueSource.Artifact);
					}
					break;
				case 12: // 月球车模型
					if(dir.Equals(new Vector2I(1, 0))){
						ChangePlayerStatus("HP", 2, ValueSource.Artifact);
					}
					break;
				case 17: // 金萝卜
					var res = itemManager.GetReachableItems(characterManager.Player.Coord, ItemData.Type.Resource);
					foreach(var r in res){
						r.value += 1;
					}
					break;
			}
		}
	}

	private void ProcessArtifactsBeforeBattle(Character enemy){
		foreach(var a in playerStatus.artifacts){
			switch(a.id){
				case 1: // 博丽护符
					DamageEnemy(enemy, 4, ValueSource.Artifact);
					break;
			}
		}
	}

	private void ProcessArtifactsAfterBattle(){
		foreach(var a in playerStatus.artifacts){
			switch(a.id){
				case 2: // 阴阳玉
					foreach(var enemy in characterManager.GetReachableEnemies()){
						DamageEnemy(enemy, 3, ValueSource.Artifact);
					}
					break;
				case 16: // 西行妖之扇
					playerStatus.maxHP += 1;
					break;
			}
		}
	}

	private bool HasArtifact(int id){
		foreach(var a in playerStatus.artifacts){
			if(a.id == id) return true;
		}
		return false;
	}



	public void ProcessItem(Item item){

		var data = item.Data;

		switch(data.type){
			case ItemData.Type.Food:

				switch(data.iconName){
					case "apple":
						ChangePlayerStatus("HP", item.value, ValueSource.Food);
						break;
					case "milk":
						ChangePlayerStatus("shield", item.value, ValueSource.Food);
						break;
					case "mushroom":
						if(rng.Randf() < 0.7f) ChangePlayerStatus("HP", item.value, ValueSource.Food);
						else DamagePlayer(item.value / 2, ValueSource.Food);
						// else ChangePlayerStatus("HP", -);
						break;
				}

				break;
			case ItemData.Type.Resource:

				switch(data.iconName){
					case "magic_stone":
						ChangePlayerStatus("money", item.value, ValueSource.Resource);
					break;
				}

				break;
			case ItemData.Type.Trap:
				switch(data.iconName){
					case "bear_trap":
						DamagePlayer(item.value, ValueSource.Trap);
						// ChangePlayerStatus("HP", -item.value);
						break;
				}
				break;
			case ItemData.Type.Artifact:
				if(playerStatus.money >= item.value){
					playerStatus.artifacts.Add(data);
					ChangePlayerStatus("money", -item.value, ValueSource.Buy);
				}
				break;

			case ItemData.Type.Skill:
				if(playerStatus.money > item.value){
					playerStatus.skills.Add(data.skill);
					ChangePlayerStatus("money", -item.value, ValueSource.Buy);
				}
				break;
		}

		itemManager.DeleteObject(item);

	}

	/// <summary>
	/// 根据当前层数，改变道具数值
	/// </summary>
	/// <param name="item"></param>
	private void SetItemValue(Item item){
		
		var level = gameStatus.currentLevel;
		var depth = level / 5;

		switch(item.Data.type){
			case ItemData.Type.Food:
				item.value = item.Data.baseValue + depth + rng.RandiRange(-3, 3);
				break;
			case ItemData.Type.Trap:
				item.value = (int)((item.Data.baseValue + depth * 3) * rng.RandfRange(0.8f, 1.2f));
				break;
			case ItemData.Type.Resource:
				item.value = item.Data.baseValue + rng.RandiRange(-1, 2);
				break;
			case ItemData.Type.Artifact:
				item.value = item.Data.baseValue;
				break;
			case ItemData.Type.Skill:
				item.value = item.Data.baseValue;
				break;
		}
	}

	/// <summary>
	/// 根据当前关卡层数，改变敌人数值
	/// </summary>
	/// <param name="enemy"></param>
	private void SetEnemyValue(Character enemy){
		var depth = gameStatus.currentLevel / 5;

		enemy.HP = (int)((enemy.Data.baseHP + depth * 5)* rng.RandfRange(0.8f, 1.2f));
		if(depth > 2) enemy.HP += 5;
	}

	/// <summary>
	/// 加载关卡数据并初始化
	/// </summary>
	/// <param name="_data"></param>
	private void LoadLevel(LevelData _data){

		levelData = _data;
		grid.SetSize(levelData.mapHeight, levelData.mapWidth);

		uiManager.ShowGameStatus(gameStatus);

		// Tiles
		tileManager.LoadGrid(grid);
		tileManager.CreateAllTiles();
		tileManager.ColorAllTiles(levelData.mapTerrain);

		// 获得所有可用的格子，并随机生成物品或敌人
		var range = new List<Vector2I>(grid.range);
		
		// 去除起始和终点位置
		range.Remove(Vector2I.Zero);
		range.Remove(new Vector2I(grid.width - 1, grid.width - 1));

		#region Characters
		characterManager.LoadGrid(grid);

		// ... Player
		var playerData = staticData.playerCharacterData;
		var player = characterManager.CreateCharacter(Vector2I.Zero, playerData, false);
		
		// ... Enemies: 战斗层每个可行位置按概率生成；
		if(levelData.type == LevelData.LevelType.Normal){
			for(int i = range.Count -1; i >= 0; i--){
				var r = rng.Randf();
				if(r < Settings.P_ENEMY_NORMAL){
					var idx = rng.RandiRange(0, levelData.enemyDataList.Count - 1);
					var enemyData = GetRandomEnemy();
					var enemy = characterManager.CreateCharacter(range[i], enemyData, true);
					SetEnemyValue(enemy);
					tileManager.GetAt(range[i]).ShowEnemy(enemy);
					range.RemoveAt(i);
				}
			}
		}
		else if(levelData.type == LevelData.LevelType.Bonus){
			// 奖励层不生成
		}
		
		
		#endregion

		#region Items
		itemManager.LoadGrid(grid);

		// ...Artifact: 战斗层每个可用格子按概率生成；奖励层不生成
		// ...Items: 战斗层未生成遗物则生成；奖励层未生成技能则生成

		ItemData data;
		Item item;

		if(levelData.type == LevelData.LevelType.Normal){

			var artifactPosition = new List<Vector2I>();

			for(int i = range.Count -1; i >= 0; i--){

				var type = Utils.Draw(Settings.SPAWN_NORMAL, Settings.SPAWN_NORMAL_CP, rng);
				if(type == ItemData.Type.Artifact){
					// 标记生成遗物的位置
					artifactPosition.Add(range[i]);
				}
				else{
					data = GetRandomItem(type);
					item = itemManager.CreateItem(range[i], data);
					SetItemValue(item);
					tileManager.GetAt(range[i]).ShowItem(item);	
				}
				
				range.RemoveAt(i);
			}
			// 随机生成未拥有的遗物
			var artifacts = GetRandomArtifacts(artifactPosition.Count);
			for(int i = 0; i < artifactPosition.Count; i++){
				data = artifacts[i];
				item = itemManager.CreateItem(artifactPosition[i], artifacts[i]);
				SetItemValue(item);
				tileManager.GetAt(artifactPosition[i]).ShowItem(item);
			}
		}
		else if(levelData.type == LevelData.LevelType.Bonus){

			// 奖励层先生成技能
			var skillPos = new List<Vector2I>(){ 
				new Vector2I(grid.width - 1, grid.width - 3), 
				new Vector2I(grid.width - 3, grid.width - 1)
			};
			var skills = GetRandomSkills(2);
			for(int i = 0; i < 2; i++){
				// GD.Print(skills[i].price);
				data = new ItemData(skills[i]);
				item = itemManager.CreateItem(skillPos[i], data);
				SetItemValue(item);
				tileManager.GetAt(skillPos[i]).ShowItem(item);
			}
			foreach(var pos in range){	
				if(!skillPos.Contains(pos)){
					var type = Utils.Draw(Settings.SPAWN_BONUS, Settings.SPAWN_BONUS_CP, rng);
					data = GetRandomItem(type);
					item = itemManager.CreateItem(pos, data);
					SetItemValue(item);
					tileManager.GetAt(pos).ShowItem(item);
				}
			}
		}

		#endregion
		if(tween != null) tween.Kill();
		tween = CreateTween();
		state = SceneState.Animating;
		tween.TweenCallback(Callable.From(()=>{ 
			ProcessArtifactsLoadLevel();
			UpdateTiles();
			state = SceneState.Waiting;
		}));

	}

	/// <summary>
	/// 生成一个随机非遗物、技能的道具数据
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private ItemData GetRandomItem(ItemData.Type type){
		switch(type){

			case ItemData.Type.Food:
				return Utils.Draw(Settings.FOODS, Settings.FOODS_CP, rng);
			case ItemData.Type.Resource:
				return Utils.Draw(Settings.RESOURCES, Settings.RESOURCES_CP, rng);
			case ItemData.Type.Trap:
				return Utils.Draw(Settings.TRAPS, Settings.TRAPS_CP, rng);
			// case ItemData.Type.Artifact:
			// 	return GetRandomArtifacts(1)[0];
			// case ItemData.Type.Skill:
			// 	var skill = GetRandomSkills(1)[0];
			// 	return new ItemData(skill);

			default:
				return ItemData.DEBUG_ITEM;
		}
	}

	/// <summary>
	/// 生成一个随机敌人数据
	/// </summary>
	/// <returns></returns>
	private CharacterData GetRandomEnemy(){
		var count = levelData.enemyDataList.Count;
		var idx = rng.RandiRange(0, count - 1);
		return levelData.enemyDataList[idx];
	}

	/// <summary>
	/// 获得指定数量的玩家未拥有的技能数据。若已拥有全部技能，使用空值补缺
	/// </summary>
	/// <param name="count"></param>
	/// <returns></returns>
	private List<SkillData> GetRandomSkills(int count = 1){

		var pool = new List<SkillData>();
		pool.AddRange(staticData.allSkills.Values);
		foreach(var skill in playerStatus.skills){
			pool.Remove(skill);
		}
		if(pool.Count < count){
			for(int i = pool.Count; i < count; i++){
				pool.Add(null);
			}
			return pool;
		}
		else{
			return Utils.GetRandomChildren(pool, count, rng);
		}

		// return SkillData.System_00;
	}

	/// <summary>
	/// 获得指定数量的玩家未拥有的遗物数据。若已拥有全部遗物，使用空值补缺
	/// </summary>
	/// <param name="count"></param>
	/// <returns></returns>
	private List<ItemData> GetRandomArtifacts(int count = 1){

		var pool = new List<ItemData>();
		pool.AddRange(staticData.allArtifacts.Values);
		foreach(var a in playerStatus.artifacts){
			pool.Remove(a);
		}

		// PrintItems(pool);
		GD.Print("Creating artifacts");
		GD.Print(string.Format("Required: {0}; Pool: {1}", count, pool.Count));

		if(pool.Count < count){
			for(int i = pool.Count; i < count; i++){
				pool.Add(null);
			}
			return pool;
		}
		else{
			return Utils.GetRandomChildren(pool, count, rng);
		}
	}

}
