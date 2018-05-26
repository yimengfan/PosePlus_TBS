using System;
using System.Collections;
using System.Collections.Generic;
using BDFramework.ScreenView;
using BDFramework.UI;
using UnityEngine;

[ScreenView("Logo" )]
public class ScreenView_Logo : IScreenView
{
    public void BeginInit(Action<Exception> onInit, ScreenViewLayer layer)
    {
        this.IsLoad = true;
       //BDebug.Log("enter logo");
        
        UIMgr.Inst.LoadWindows(0);
        UIMgr.Inst.ShowWindow(0);
        
        IEnumeratorTool.WaitingForExec(3, () =>
        {
            ScreenViewMgr.Inst.BeginNav("Battle");
        });
    }

    public void BeginExit(Action<Exception> onExit)
    {
        this.IsLoad = false;
        Destory();
    }

    public void Destory()
    {
        UIMgr.Inst.CloseWindow(0);   
    }

    public void Update(float delta)
    {
    }

    public void UpdateTask(float delta)
    {
    }

    public void FixedUpdate(float delta)
    {
    }

    public string Name { get; set; }
    public bool IsLoad { get; set; }
    public bool IsBusy { get; set; }
    public bool IsTransparent { get; set; }
    
    
 
}