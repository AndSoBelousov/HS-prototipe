using Cards;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoosingHero1 : MonoBehaviour
{   
    [SerializeField]
    private Transform _positionFirstPlayer;
    [SerializeField]
    private Transform _positionSecondPlayer;
    [SerializeField]
    private GameObject _panalsUI;
    private bool _pause = false;
    private bool _isFirstPlayerSelected = false;
  

    public void MovingIcon(GameObject obj)
    {
        Transform endPos = _isFirstPlayerSelected == false ? _positionFirstPlayer.transform : _positionSecondPlayer.transform;

        
        obj.transform.SetParent(endPos);

        obj.transform.localPosition = Vector3.zero; // Относительно нового родителя
        obj.transform.localRotation = Quaternion.identity; // Относительно нового родителя
        obj.transform.localScale = Vector3.one; // Относительно нового родителя

        

        if (_isFirstPlayerSelected == true)
        {
            FindFirstObjectByType<CardManager>().StartGame();
            _panalsUI.SetActive(false);
        }

        _isFirstPlayerSelected = true;
    }


 
}
