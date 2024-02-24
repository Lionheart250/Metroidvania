using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
    private GrassVelocityController _grassVelocityController;

    private GameObject _player;

    private Material _material;
    private Rigidbody2D _playerRB;
    private bool _easeInCoroutineRunning;
    private bool _easeOutCoroutineRunning;

    private int _externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    private float _startingXVelocity;
    private float _velocityLastFrame; 

    private void Start() 
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRB = _player.GetComponent<Rigidbody2D>();
        _grassVelocityController = GetComponentInParent<GrassVelocityController>();

        _material = GetComponent<SpriteRenderer>().material;
        _startingXVelocity = _material.GetFloat(_externalInfluence);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player && gameObject.activeSelf)
        {
            if(!_easeInCoroutineRunning && Mathf.Abs(_playerRB.velocity.x) > Mathf.Abs(_grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(_playerRB.velocity.x * _grassVelocityController.ExternalInfluenceStrength));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _player && gameObject.activeSelf)
        {
            StartCoroutine(EaseOut());
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == _player && gameObject.activeSelf)
        {
            if (Mathf.Abs(_velocityLastFrame) > Mathf.Abs(_grassVelocityController.VelocityThreshold) &&
            Mathf.Abs(_playerRB.velocity.x) < Mathf.Abs(_grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseOut());
            }

            else if (Mathf.Abs(_velocityLastFrame) < Mathf.Abs(_grassVelocityController.VelocityThreshold) &&
            Mathf.Abs(_playerRB.velocity.x) > Mathf.Abs(_grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(_playerRB.velocity.x * _grassVelocityController.ExternalInfluenceStrength));
            }

            else if (!_easeInCoroutineRunning && !_easeOutCoroutineRunning &&
            _playerRB.velocity.x > _grassVelocityController.VelocityThreshold)
            {
                _grassVelocityController.InfluenceGrass(_material, _playerRB.velocity.x * _grassVelocityController.ExternalInfluenceStrength);
            }

            _velocityLastFrame = _playerRB.velocity.x;
        }
        
    }



    private IEnumerator EaseIn(float XVelocity)
    {
        _easeInCoroutineRunning = true;

        float elapsedTime = 0f;
        float damping = 0.6f; // Adjust damping factor to control the smoothness of return
        while(elapsedTime < _grassVelocityController.EaseInTime)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / _grassVelocityController.EaseInTime;
            float lerpedAmount = Mathf.Lerp(_startingXVelocity, XVelocity, t);
            float dampedAmount = Mathf.Lerp(lerpedAmount, _startingXVelocity, Mathf.Pow(t, damping));
            _grassVelocityController.InfluenceGrass(_material, dampedAmount);

            yield return null;
        }

        _easeInCoroutineRunning = false;
    }

    private IEnumerator EaseOut()
    {
        _easeOutCoroutineRunning = true;
        float currentXInfluence = _material.GetFloat(_externalInfluence);

        float elapsedTime = 0f;
        float damping = 0.6f; // Adjust damping factor to control the smoothness of return
        while(elapsedTime < _grassVelocityController.EaseOutTime)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / _grassVelocityController.EaseOutTime;
            float lerpedAmount = Mathf.Lerp(currentXInfluence, _startingXVelocity, t);
            float dampedAmount = Mathf.Lerp(lerpedAmount, _startingXVelocity, Mathf.Pow(t, damping));
            _grassVelocityController.InfluenceGrass(_material, dampedAmount);

            yield return null;
        }

        _easeOutCoroutineRunning = false;
    }

}
