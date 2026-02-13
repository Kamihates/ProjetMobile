using System.Globalization;
using UnityEngine;

public class FrozenEffect : BossEffect
{
    protected override void OnActivated()
    {
        if (GridManager.Instance ==  null) return;

        int effectTry = 0;

        while (true)
        {

            if (effectTry >= 50) // on a le droit a 50 essaies
                return; 


            effectTry ++;
            int randowRow = Random.Range(1, GridManager.Instance.Row);
            int randowCol = Random.Range(0, GridManager.Instance.Column);

            if (GridManager.Instance.GridData[randowRow][randowCol] == null)
            {
                // on bloque la position de la grille
                Debug.Log("FREEZE");
                GridManager.Instance.DisableCell(new Vector2Int(randowRow, randowCol));
                Deactivate();
                return;
            }
        }
        



    }

    protected override void OnDeactivated()
    {

    }
}
