using Godot;
using System;
using System.Collections.Generic;

public partial class ArtifactsUI : Control
{
    [Export] private PackedScene artifactPrefab;
    [Export] private Control artifactParent;
    private List<ArtifactIcon> artifacts;

    public override void _Ready()
    {
        artifacts = new List<ArtifactIcon>();
    }

    public ArtifactIcon CreateArtifactIcon(ItemData artifactData){
        var obj = artifactPrefab.Instantiate<ArtifactIcon>();
        artifactParent.AddChild(obj);
        artifacts.Add(obj);

        obj.LoadData(artifactData);

        return obj;
    }

    public void DeleteArtifactIcon(ArtifactIcon icon){
        artifacts.Remove(icon);
        icon.QueueFree();
    }

    public void ClearArtifactIcons(){
        foreach(var icon in artifacts){
            icon.QueueFree();
        }
        artifacts = new List<ArtifactIcon>();
    }

    public void ShowArtifacts(List<ItemData> datas){

        if(artifacts == null) artifacts = new List<ArtifactIcon>();

        if(artifacts.Count > datas.Count)
            ClearArtifactIcons();

        for(int i = 0; i < artifacts.Count; i++){
            artifacts[i].LoadData(datas[i]);
        }

        for(int i = artifacts.Count; i < datas.Count; i++){
            CreateArtifactIcon(datas[i]);
        }

    }
}
