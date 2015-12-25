using UnityEngine;
using System.Collections;

public class ParticleEmitterTest : MonoBehaviour {

	public GameObject particle;
	
	void Update () {
		if(GetComponent<ParticleSystem>() != null) {
			if(Input.GetKeyDown(KeyCode.E)) {
				ParticleSystem[] particleSystems = particle.GetComponentsInChildren< ParticleSystem >();
				ParticleBurstQuantity[] particleBurstQuantities = particle.GetComponentsInChildren< ParticleBurstQuantity >();
				
				if (particleSystems.Length != particleBurstQuantities.Length) {
					Debug.LogWarning("You have forgot to add the ParticleBurstQuantity sript on the particleSystem: " + particle.name + " you should only use Emit");
				}
					
				for (int i = 0;  i< particleSystems.Length; i++) {
					particleSystems[i].transform.position = transform.position;
					particleSystems[i].Emit(particleBurstQuantities[i].burstQuantity);	
				}
			}
		}
	}
}
