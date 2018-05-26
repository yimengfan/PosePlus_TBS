using System;
using System.Collections;
using System.Collections.Generic;
using BDFramework.ScreenView;
using UnityEngine;

[ScreenView("Battle")]
public class ScreenView_Battle : IScreenView
{
    public void BeginInit(Action<Exception> onInit, ScreenViewLayer layer)
    {
        this.IsLoad = true;
    }

    public void BeginExit(Action<Exception> onExit)
    {
        this.IsLoad = false;
        Destory();
    }

    public void Destory()
    {
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