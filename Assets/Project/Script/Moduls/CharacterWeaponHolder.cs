using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace TopDown_Template
{
    public class CharacterWeaponHolder : WeaponHolder
    {
        #region Variable

        [SerializeField] private Transform _parent;
        [SerializeField] private SpriteRenderer _secondWeapon;
        [SerializeField] private WeaponContainer _prefabContainer;

        [Header("ChangeWeapon")]
        [SerializeField] private List<Weapon> _weaponList = new List<Weapon>();
        private int _currentWeaponIndex = 0;
        #endregion

        #region Getter Setter

        public int CurrentWeaponIndex { get => _currentWeaponIndex; set => _currentWeaponIndex = value; }
        public List<Weapon> WeaponList { get => _weaponList; set => _weaponList = value; }
        #endregion
        
        #region UnityCallback
        protected override void Awake()
        {
            base.Awake();
            if (_weapon)
            {
                _weaponList.Insert(0, _weapon);
            }
            for (int i = 0; i < _weaponList.Count; i++)
            {
                SetupWeapon(_weaponList[i]);
            }
            SetSecondWeaponVisual();
        }
        #endregion
        #region Overrider Method WeaponHolder
        public override void SetupWeapon(Weapon weapon)
        {
            base.SetupWeapon(weapon);


        }
        public override void SwitchWeapon()
        {

            _currentWeaponIndex++;
            if (_currentWeaponIndex >= _weaponList.Count)
            {
                _currentWeaponIndex = 0;
            }
            if (_weaponList.Count > 1)
            {
                SetSecondWeaponVisual();
            }

            if (_weaponList.Count > 1)
            {
                if (!_weapon.gameObject.activeSelf) return;
                if (_weapon)
                {
                    _weapon.SwitchWeapon();
                    _weapon.gameObject.SetActive(false);
                }

                switch (_currentWeaponIndex)
                {
                    case 0:
                        if (_weaponList[0] != null)
                        {
                            _weapon = _weaponList[0];
                        }
                        break;
                    case 1:
                        if (_currentWeaponIndex < _weaponList.Count && _weaponList[1] != null)
                        {
                            _weapon = _weaponList[1];
                        }
                        break;

                    case 2:
                        if (_currentWeaponIndex < _weaponList.Count && _weaponList[2] != null)
                        {
                            _weapon = _weaponList[2];
                        }
                        break;
                }
                _weapon.gameObject.SetActive(true);
                _weapon.SwitchWeapon();
                SwitchWeaponEvent?.Invoke();
            }

        }
        #endregion

        #region CharacterWeaponHolder Method
        public void SetSecondWeaponVisual()
        {
            if (_weaponList.Count > 1)
            {
                int temp = _currentWeaponIndex;
                if (temp + 1 >= _weaponList.Count)
                {
                    temp = 0;
                }
                else
                {
                    temp++;
                }
                // Here we are setting the back weapon sprite for the player => Modified to place the Sprite back so we can have different sprites
                _secondWeapon.sprite = _weaponList[temp].SpriteBack;

            }
        }
        public bool RestoreAmmo(int value)
        {
            if (_weapon.GetStockAndMagAmmo().Item1 < _weapon.GetStockAndMagMax().Item1)
            {
                _weaponList[_currentWeaponIndex].RestoreAmmo(value);
                RestoreAmmoEvent?.Invoke();
                return true;

            }
            else
            {
                return false;
            }
        }
        public bool ExchangeWeapon(Weapon weapon, (int, int) ammo, out Weapon backWeapon)//����� � ����������� 
        {
            backWeapon = null;
            if (_weaponList.Count >= 3)
            {
                Debug.Log(1);
                return false;
            }
            else
            {
                if (_weaponList.Count == 2)
                {
                    backWeapon = _weaponList[_currentWeaponIndex];
                    _weaponList.Remove(backWeapon);
                }

                Weapon go = Instantiate(weapon, _parent);
                go.name = weapon.name;
                _weaponList.Insert(_currentWeaponIndex, go);

                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                if (ammo.Item1 == -1 && ammo.Item2 == -1)
                {
                    go.SetupWeapon(this);

                }
                else
                {
                    go.SetupWeapon(this, false);
                    go.SetStockAndMagAmmo(ammo);

                }

                if (_weaponList.Count == 2)
                {
                    _weapon.gameObject.SetActive(false);
                    _weapon = go;
                    go.gameObject.SetActive(true);
                    SetSecondWeaponVisual();

                    TakeWeaponEvent?.Invoke(go, backWeapon);
                    SetupWeaponEvent?.Invoke();

                }
                else
                {

                    TakeWeaponEvent?.Invoke(go, backWeapon);
                    go.gameObject.SetActive(false);
                }

                return true;
            }
        }
        public bool TakeWeapon(Weapon weapon, (int, int) ammo, out Weapon backWeapon, out WeaponContainer prefabContainer) //������� �� ���������� 
        {
            backWeapon = null;
            prefabContainer = null;
            if (_weaponList.Count >= 3)
            {

                return false;
            }
            else
            {
                prefabContainer = Instantiate(_prefabContainer, transform.position, Quaternion.identity);
                if (_weaponList.Count == 2)
                {
                    backWeapon = _weaponList[_currentWeaponIndex];
                    _weaponList.Remove(backWeapon);
                }

                Weapon go = Instantiate(weapon, _parent);
                go.name = weapon.name;
                _weaponList.Insert(_currentWeaponIndex, go);

                go.transform.localPosition = Vector3.zero;
                if (ammo.Item1 == -1 && ammo.Item2 == -1)
                {
                    go.SetupWeapon(this);

                }
                else
                {
                    go.SetupWeapon(this, false);
                    go.SetStockAndMagAmmo(ammo);


                }

                if (_weaponList.Count == 2)
                {
                    _weapon.gameObject.SetActive(false);
                    _weapon = go;
                    go.gameObject.SetActive(true);
                    SetSecondWeaponVisual();

                    TakeWeaponEvent?.Invoke(go, backWeapon);
                    SetupWeaponEvent?.Invoke();

                }
                else
                {

                    TakeWeaponEvent?.Invoke(go, backWeapon);
                    go.gameObject.SetActive(false);
                }
                return true;

            }
        }
        #endregion
       

    }
}