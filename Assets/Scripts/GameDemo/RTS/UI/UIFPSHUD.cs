using UnitedSolution;using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class UIFPSHUD : MonoBehaviour {

		public Text txtAmmo;
		public Text txtReload;
		public Image spriteWeaponIcon;
		
		public RectTransform rectRecticleSpread;
		
		private GameObject thisObj;
		private static UIFPSHUD instance;
		
		void Awake(){
			instance=this;
			thisObj=gameObject;
			
			txtReload.text="";
		}
		
		// Use this for initialization
		void Start () {
			Hide();
		}
		
		// Update is called once per frame
		void Update () {
			float value=FPSControl.GetRecoilModifier();
			value=Mathf.Min(value*25, 250);
			rectRecticleSpread.sizeDelta=new Vector2(150, 150)+new Vector2(value, value)*2;
		}
		
		void OnEnable(){
			FPSControl.onFPSShootE += UpdateAmmoCount;
			FPSControl.onFPSReloadE += OnFPSReload;
			FPSControl.onSwitchWeaponE += OnSwitchWeapon;
		}
		void OnDisable(){
			FPSControl.onFPSShootE -= UpdateAmmoCount;
			FPSControl.onFPSReloadE -= OnFPSReload;
			FPSControl.onSwitchWeaponE -= OnSwitchWeapon;
		}
		
		void UpdateAmmoCount(){
			int total=FPSControl.GetTotalAmmoCount();
			int current=FPSControl.GetCurrentAmmoCount();
			txtAmmo.text=current+"/"+total;
		}
		
		private bool reloading=false;
		void OnFPSReload(bool flag){
			reloading=flag;
			if(reloading) StartCoroutine(ReloadRoutine());
			else{
				txtReload.text="";
				UpdateAmmoCount();
			}
		}
		IEnumerator ReloadRoutine(){
			txtReload.text="Reloading";
			int count=0;
			while(reloading){
				string text="";
				for(int i=0; i<count; i++) text+=".";
				
				txtReload.text="Reloading"+text;
				
				count+=1;
				if(count==4) count=0;
				yield return new WaitForSeconds(0.25f);
			}
			txtReload.text="";
		}
		
		
		void OnSwitchWeapon(){
			UpdateAmmoCount();
			reloading=false;
			
			Sprite weapIcon=FPSControl.GetCurrentWeaponIcon();
			spriteWeaponIcon.sprite=weapIcon;
		}
		
		
		public static bool isOn=true;
		public static void Show(){ instance._Show(); }
		public void _Show(){
			OnSwitchWeapon();
			isOn=true;
			thisObj.SetActive(isOn);
		}
		public static void Hide(){ instance._Hide(); }
		public void _Hide(){
			isOn=false;
			thisObj.SetActive(isOn);
		}
		
	}

}