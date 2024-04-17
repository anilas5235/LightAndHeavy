using System;
using System.ComponentModel;
using UnityEngine;

namespace AttributesLibrary.SceneSelect
{
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field)]
    public class SceneSelect : PropertyAttribute
    { }
}