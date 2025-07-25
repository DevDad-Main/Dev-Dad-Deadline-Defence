using System;
using UnityEngine;
namespace TopDown_Template
{
    public class WeaponHolderUi : MonoBehaviour
    {
        #region Variable
        [SerializeField] private CharacterWeaponHolder _weaponHodler;
        [SerializeField] private WeaponCardUi[] _cardsWeapon;
        [SerializeField] private Transform[] _cardsPosition;
        #endregion
        #region UnityCallback
        private void Start()
        {
            Construnct(_weaponHodler);

        }
        #endregion
        #region WeaponHolderUi Method
        private void Construnct(CharacterWeaponHolder weaponHodler)
        {
            _weaponHodler = weaponHodler;
            weaponHodler.SwitchWeaponEvent.AddListener(SetupCard);
            weaponHodler.SetupWeaponEvent.AddListener(SetupCard);
            weaponHodler.RestoreAmmoEvent.AddListener(UpdateAmmo);
            weaponHodler.UseWeaponEvent.AddListener(UpdateAmmo);
            weaponHodler.ReloadingWeaponEvent.AddListener(UpdateAmmo);
            weaponHodler.EndReloadingWeaponEvent.AddListener(UpdateAmmo);
            SetupCard();
        }
        private void SetupCard()
        {

            int secondIndex = _weaponHodler.CurrentWeaponIndex == 1 ? 0 : 1;
            int firstIndex = _weaponHodler.CurrentWeaponIndex;
            if (_weaponHodler.WeaponList.Count > 0)
            {
                if (_weaponHodler.WeaponList[firstIndex] != null)
                {
                    _cardsWeapon[0].gameObject.SetActive(true);
                    _cardsWeapon[0].SetCard(_weaponHodler.WeaponList[firstIndex].SpriteOutline, _weaponHodler.WeaponList[firstIndex].name, _weaponHodler.WeaponList[firstIndex].GetStockAndMagAmmo().Item2 + "", _weaponHodler.WeaponList[firstIndex].GetStockAndMagAmmo().Item1 + "");
                }
                if (_weaponHodler.WeaponList.Count > 1)
                {
                    _cardsWeapon[1].gameObject.SetActive(true);
                    _cardsWeapon[1].SetCard(_weaponHodler.WeaponList[secondIndex].SpriteOutline, _weaponHodler.WeaponList[secondIndex].name, _weaponHodler.WeaponList[secondIndex].GetStockAndMagAmmo().Item2 + "", _weaponHodler.WeaponList[secondIndex].GetStockAndMagAmmo().Item1 + "");
                }
                else
                {
                    _cardsWeapon[1].gameObject.SetActive(false);
                }
            }
            else
            {
                _cardsWeapon[1].gameObject.SetActive(false);
                _cardsWeapon[0].gameObject.SetActive(false);
            }
        }
        private void UpdateAmmo()
        {
            _cardsWeapon[0].UpdateAmmo(_weaponHodler.WeaponList[_weaponHodler.CurrentWeaponIndex].GetStockAndMagAmmo().Item2 + "", _weaponHodler.WeaponList[_weaponHodler.CurrentWeaponIndex].GetStockAndMagAmmo().Item1 + "");
        }
        #endregion

    }
}
