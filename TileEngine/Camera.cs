﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
    public class Camera
    {
        float speed = 5;

        public Vector2 Position = Vector2.Zero;
        
        public float Speed
        {
            get { return speed; }
            set { speed = (float) Math.Max(value, 1f); }
        }
        public void Update()
        {

            KeyboardState keyState = Keyboard.GetState(); // return state of entire keyboard
            Vector2 motion = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.Up))
                motion.Y--;
            if (keyState.IsKeyDown(Keys.Down))
                motion.Y++;
            if (keyState.IsKeyDown(Keys.Left))
                motion.X--;
            if (keyState.IsKeyDown(Keys.Right))
                motion.X++;


            if (motion != Vector2.Zero)
            {
                motion.Normalize();
                Position += motion * Speed;
            }
        }
    }
}