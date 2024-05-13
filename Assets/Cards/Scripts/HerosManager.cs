using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerosManager : MonoBehaviour
{

    private int _healthHero = 30;
    private int _maxManaCrystals= 0;
    private int _currentManaCrystals = 0;
    private bool _isHeroOneSelect = false;

    public bool IsHeroOneSelect
    { get { return _isHeroOneSelect; } set { _isHeroOneSelect = value; } }

}
