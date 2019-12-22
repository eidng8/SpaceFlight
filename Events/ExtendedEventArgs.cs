﻿// ---------------------------------------------------------------------------
// <copyright file="ExtendedEventArgs.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System;
using UnityEngine;

namespace eidng8.SpaceFlight.Events
{
    [Serializable]
    public class ExtendedEventArgs : EventArgs
    {
        public GameObject Source = null;
        public GameObject Target = null;
    }
}
