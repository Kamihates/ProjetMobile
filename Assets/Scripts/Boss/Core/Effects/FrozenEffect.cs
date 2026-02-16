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


            int randomRow = Random.Range(2, GridManager.Instance.Row);
            int randomCol = Random.Range(0, GridManager.Instance.Column);

            if (GridManager.Instance.GridData[randomRow][randomCol] == null)
            {
                // on bloque la position de la grille
                GridManager.Instance.DisableCell(new Vector2Int(randomRow, randomCol));
                Deactivate();
                return;
            }
        }
        



    }

    protected override void OnDeactivated()
    {

    }
}
