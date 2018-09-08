using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeChanger : MonoBehaviour
{
	[SerializeField] private UISprite hpBar;
	[SerializeField] private UISprite atkBar;
	[SerializeField] private UISprite defBar;
	[SerializeField] private UISprite armoBar;
	[SerializeField] private UISprite rldBar;
	
	private Coroutine gaugeCoroutine;
	
	void Start ()
	{
		SetGaugeChanger();
	}
	
	private void SetGaugeChanger()
	{
		if (this.gaugeCoroutine != null)            
			StopCoroutine(this.gaugeCoroutine);
		this.gaugeCoroutine = StartCoroutine(GaugeEnumerator());
	}

	private IEnumerator GaugeEnumerator()
	{
		float targetHp = 100f * 0.01f * 5;
		float targetAtack = 100f * 0.01f * 50;
		float targetDefence = 100f * 0.01f * 50;
		float targetArmo = 100f * 0.01f * 50;
		float targetReload = 100f * 0.01f * 50;

		this.hpBar.fillAmount = 0.0f;
		this.atkBar.fillAmount = 0.0f;
		this.defBar.fillAmount = 0.0f;
		this.armoBar.fillAmount = 0.0f;
		this.rldBar.fillAmount = 0.0f;

		bool proc = true;

		while (proc)
		{
			proc = false;
			if (targetHp > this.hpBar.fillAmount)
			{
				this.hpBar.fillAmount += 0.01f;
				proc = true;
			}
			if (targetAtack > this.atkBar.fillAmount)
			{
				this.atkBar.fillAmount += 0.01f;
				proc = true;
			}
			if (targetDefence > this.defBar.fillAmount)
			{
				this.defBar.fillAmount += 0.01f;
				proc = true;
			}
			if (targetArmo > this.armoBar.fillAmount)
			{
				this.armoBar.fillAmount += 0.01f;
				proc = true;
			}
			if (targetReload > this.rldBar.fillAmount)
			{
				this.rldBar.fillAmount += 0.01f;
				proc = true;
			}

			yield return new WaitForSeconds(0.01f);
		}
	}
}
