﻿using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [SerializeField] private Material _playerMat = null;
    [SerializeField] private float _cooldown = 0.35f;
    [SerializeField] protected float _castTime = 0.1f;
    [Tooltip("Base movement is 1.0f, slow amount divides that by [value] amount (additive to other slows)")]
    [SerializeField] private float _movementSlowAmount = 2.0f; 
    [SerializeField] private int _maxCharges = 1;
    private PlayerMovement _movement = null;
    private PlayerAttacks _attacks = null;
    private float _cooldownTimer = 0.0f;
    protected float _castTimer = 0.0f;
    private bool _isCasting = false;
    private int _charges = 1;

    public void ManualUpdate()
    {
        if (!_isCasting)
            UpdateNotCasting();
        else
            UpdateCasting();
    }

    private void Start()
    {
        _movement = GetComponentInParent<PlayerMovement>();
        _attacks = GetComponentInParent<PlayerAttacks>();
    }

    public bool AttemptInitiate()
    {
        if (_charges == 0) return false; //TODO: Cooldown animation? Or should be handled elsewhere?
        _isCasting = true;
        _movement.moveSlow += _movementSlowAmount;
        _castTimer = 0.0f;
        --_charges;
        if (_cooldownTimer < 0.0f) _cooldownTimer = _cooldown;
        return true;
    }

    private void UpdateCasting()
    {
        _castTimer += Time.deltaTime;
        Color playerCol = _playerMat.GetColor("_Color");
        playerCol.g = _castTimer / _castTime;
        playerCol.r = 0.0f;
        playerCol.b = playerCol.r;
        _playerMat.SetColor("_Color", playerCol);
        if (_castTimer > _castTime)
            OnCastFinish();
    }

    protected abstract void OnCastFinish();

    public void Cancel()
    {
        _movement.moveSlow -= _movementSlowAmount;
        _attacks.activeAttackIDX = -1;
        _isCasting = false;
        _playerMat.SetColor("_Color", Color.white);
    }

    protected void UpdateNotCasting()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_charges < _maxCharges && _cooldownTimer < 0.0f)
        {
            ++_charges;
            _cooldownTimer += _cooldown;
        }
    }
}
