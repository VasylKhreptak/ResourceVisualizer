using System;
using Infrastructure.Services.PersistentData.Core;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Test
{
    public class CoinsText : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _tmp;

        private IPersistentDataService _persistentDataService;

        [Inject]
        private void Constructor(IPersistentDataService persistentDataService) => _persistentDataService = persistentDataService;

        private IDisposable _subscription;

        #region MonoBehaviour

        private void OnValidate() => _tmp ??= GetComponent<TMP_Text>();

        private void OnEnable() => _subscription = _persistentDataService.Data.PlayerData.Coins.Amount.Subscribe(UpdateText);

        private void OnDisable() => _subscription?.Dispose();

        #endregion

        private void UpdateText(int coins) => _tmp.text = coins.ToString();
    }
}