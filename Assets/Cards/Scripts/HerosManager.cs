using UnityEngine;

public class HerosManager
{
    private int _healthHero = 30;
    private int _maxManaCrystals = 0;
    private int _currentManaCrystals = 0;
    private bool _isHeroOneSelect = false;

    public bool IsHeroOneSelect
    { get { return _isHeroOneSelect; } set { _isHeroOneSelect = value; } }

}
