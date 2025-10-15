using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public GameObject canExecute {  get; set; }
    public Vector3 executionOffset {  get; set; }
    public bool isGrounded {  get; set; }
    public Animator _Animator { get; }
    public void GetExecuted(int random);
    public void Revive();
}
