//  Importacions
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Clase de les cartes
public class Card : MonoBehaviour
{
    //  Creem les variables de tipus 'GameObject' de les figures i del GameManager.
    public GameObject figura;
    private GameObject gm;

    //  Creem la variable per asignar la 'Id' a cada carta.
    private int id;

    //  Creem la variable de tipus 'Material' on és trobara l'matge. 
    public Material image;

    //  Creem la variable de tipus boolea el qual indicara l'estat del botó de inici de joc.
    public static bool startVar;

    //  Constructor de 'Start'
    void Start()
    {
        //  Inicializem la variable com a false, per evitar que s'activi en el moment incorrecte.
        startVar=false;

        //  Fem la busqueda del 'GameManger' mitjançant el Tag:GameController.
        gm = GameObject.FindGameObjectWithTag("GameController"); 
        Renderer renderer = figura.GetComponent<Renderer>();
        renderer.material = image;
    }

    //  Constructor de 'Update'
    void Update()
    {
        //  No fem res dins del constructor 'Update'.
    }

    //  Creem la funció per controlar que succeix que és fa click sobre una carta.
    void OnMouseDown()
    {
        // Mitjançant un condicional, comprovem si ha iniciat el joc.
        if (!startVar)
        {
            return; // En el cas que no, no executem cap codi més.
        }

        // Comprovem si es pot fer clic sobre les cartes mitjançant el canClickTrigger().
        if (gm.GetComponent<GameManager>().canClickTrigger())
        {
            // En el cas que si és pugui, indiquem l'estat 'true' mitjançant el setClickTrigger().
            gm.GetComponent<GameManager>().setClickTrigger(true);

            // Ara amb aquesta variable obtindrem l'estat actual de l'animator.
            AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

            // Una vegada comprovat l'estat del animator, comprovarem si la carta està en l'estat 'CardDown' (carta amagada).
            if (stateInfo.IsName("CardDown"))
            {

                // En el cas de que la comprovació anterior hagi donat 'true' continuarem amb el procés
                if (gm.gameObject.GetComponent<GameManager>().SeleccioCartes())
                {

                    // Una altre vegada comprovarem si la carta està en l'estat 'CardDown' (carta amagada).
                    if (stateInfo.IsName("CardDown"))
                    {

                        // Una vegada fet aixó, accedim a l'animator per a activar el 'trigger' que mostrarà la carta.
                        Animator an = GetComponent<Animator>();
                        an.SetTrigger("TriggerCardShow");
                    }

                    // I indiquem al GameManager quina carta s'ha seleccionat.
                    gm.GetComponent<GameManager>().CartaSeleccionada(gameObject);
                }
            }
        }
    }

    // Creem la funció per fer l'animació de amagat.
    public void amagar(){

        // Creem aquesta variable per obtindre l'estat actual de l'animator.
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        // Una vegada comprovat l'estat del animator, comprovarem si la carta està en l'estat 'CardUp' (carta descoberta).
        if (stateInfo.IsName("CardUp"))
        {
            // Una vegada fet aixó, accedim a l'animator per a activar el 'trigger' que ocultara la carta.
            Animator an = GetComponent<Animator>();
            an.SetTrigger("TriggerCardHide");
        }
    }

    // Creem la funció per fer l'animació de ensenyar la carta.
    public void showFigure(){

        // Creem aquesta variable per obtindre l'estat actual de l'animator.
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        // Una altre vegada comprovarem si la carta està en l'estat 'CardDown' (carta amagada).
        if (stateInfo.IsName("CardDown"))
        {
            // Una vegada fet aixó, accedim a l'animator per a activar el 'trigger' que mostrarà la carta.
            Animator an = GetComponent<Animator>();
            an.SetTrigger("TriggerCardShow");
        }
    }

    //  Setters i getters sobre el maneig de les cartes.
    public Renderer getFiguraRenderer(){
        Renderer renderer = figura.GetComponent<Renderer>();
        return renderer;
    }

    public void setStartVar(bool var){
        startVar=var;
    }

    public void setId(double idp){
        id=(int)idp;
    }

    public int getId(){
        return id;
    }
    
}
