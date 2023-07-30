using System;
using System.Collections;
using TMPro;
using Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI_canvas{
    public class PointUI : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI RedText;
        [SerializeField] public TextMeshProUGUI BlueText;
        EntityManager entityManager;
        void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //_playerEntity = _entityManager.CreateEntityQuery(typeof(PointComponent)).GetSingletonEntity();
        }
        void Update() {
            if(!entityManager.CreateEntityQuery(typeof(PointComponent)).IsEmpty){
                Entity entity = entityManager.CreateEntityQuery(typeof(PointComponent)).GetSingletonEntity();
                var point = entityManager.GetComponentData<PointComponent>(entity);

                RedText.text = $"Machine: {point.RedPoint}"; 
                BlueText.text = $"Player: {point.BluePoint}"; 
            }
        }
    }
}
