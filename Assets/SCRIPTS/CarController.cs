using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameSignals;

public class CarController : MonoBehaviour {

    public List<WheelCollider> throttleWheels = new List<WheelCollider>();
    public List<WheelCollider> steeringWheels = new List<WheelCollider>();
    public float throttleCoefficient = 20000f;
    public float maxTurn = 20f;
    float giro = 0f;
    float acel = 0f;

    private bool isAccelAllowed = false;

    private Action calibrationHandler;
    private Action matchStartedHandler;
    private Action matchEndedHandler;

    private void OnEnable()
    {
        calibrationHandler = () => isAccelAllowed = false;
        matchStartedHandler = () => isAccelAllowed = true;
        matchEndedHandler = () => isAccelAllowed = false;

        CalibrationStarted += calibrationHandler;
        MatchStarted += matchStartedHandler;
        MatchEnded += matchEndedHandler;
    }

    private void OnDisable()
    {
        CalibrationStarted -= calibrationHandler;
        MatchStarted -= matchStartedHandler;
        MatchEnded -= matchEndedHandler;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isAccelAllowed) return;
        foreach (var wheel in throttleWheels) {
            wheel.motorTorque = throttleCoefficient * T.GetFDT() * acel;
        }
        foreach (var wheel in steeringWheels) {
            wheel.steerAngle = maxTurn * giro;
        }
        giro = 0f;
    }

    public void SetGiro(float giro) {
        this.giro = giro;
    }
    public void SetAcel(float val) {
        acel = val;
    }
}
