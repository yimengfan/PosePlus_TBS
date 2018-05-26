using System.Collections;
using System.Collections.Generic;
using BDFramework.Logic.GameLife;
using UnityEngine;

namespace Game
{
    public class GameStart : IGameStart
    {
        public void Start()
        {

        }

        public void Awake()
        {
            Client.Init();
        }

        public void Update()
        {

        }
    }
}
