using UnityEngine;

public class AttackRecieve : MonoBehaviour
{
    //
    private bool tryAttack;

    //
    private async Awaitable StopAttack()
    {
        //
        await Awaitable.MainThreadAsync();
        await Awaitable.WaitForSecondsAsync(0.1f);
        tryAttack = false;
    }

    //
    async public void TryAttack()
    {
        tryAttack = true;
        await StopAttack();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && tryAttack)
        {
            //
            PlayerController.Health -= Random.Range(5f, 15f);
            tryAttack = false;
        }
    }
}
