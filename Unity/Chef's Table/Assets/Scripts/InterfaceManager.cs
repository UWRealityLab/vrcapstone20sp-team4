using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    GameObject nearInterface;
    GameObject simulationInterface;
    GameObject onboardingInterface;
    GameObject cuttingSimulation;
    GameObject floatingInterface;

    // Start is called before the first frame update
    void Start()
    {
        nearInterface = GameObject.Find("NearInterface");
        simulationInterface = GameObject.Find("SimulationInterface");
        onboardingInterface = GameObject.Find("OnBoardingInterface");
        cuttingSimulation = GameObject.Find("CuttingSimulation");
        floatingInterface = GameObject.Find("Interf");

        setActiveFloatingInterface(false);
        setActiveNearInterface(false);
        setActiveSimulationInterface(false);
        setActiveCuttingSimulation(false);
        setActiveOnboardingInterface(true);
    }

    public void setActiveCuttingSimulation(bool b)
    {
        cuttingSimulation.SetActive(b);
    }

    public void setActiveNearInterface(bool b)
    {
        nearInterface.SetActive(b);
    }

    public void setActiveSimulationInterface(bool b)
    {
        simulationInterface.SetActive(b);
    }

    public void setActiveOnboardingInterface(bool b)
    {
        onboardingInterface.SetActive(b);
    }

    public void setActiveFloatingInterface(bool b)
    {
        floatingInterface.SetActive(b);
    }

    public bool isActiveSimulationInterface()
    {
        return simulationInterface.activeSelf;
    }

    public bool isActiveCuttingSimulation()
    {
        return cuttingSimulation.activeSelf;
    }

    public bool isActiveOnboardingInterface()
    {
        return onboardingInterface.activeSelf;
    }

    public bool isActiveNearInterface()
    {
        return nearInterface.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
