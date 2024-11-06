//  Importacions
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq.Expressions;
using UnityEngine;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

//  Clase principal
public class GameManager : MonoBehaviour
{
    // Creem les llistes de referencies de les cartes i els sons de le les cartes.
    public GameObject[] cards;
    public GameObject[] cardSons;

    //  Creem la variable de Id per asignar-li a les parelles de cartes
    private double id_double=0;

    // Creem la llista de referencies de les dues cartes seleccionades
    private GameObject[] cardsSelected = new GameObject[2];

    //  Creem la variable per les cartes adivinades.
    private int cartesAdivinades;

    //  Creem les variables per les quals permetre clicar les cartes, inciar el joc
    //  i el que controla un temps de retras per activar el permis de clicar cartes el permis.
    private bool clickTrigger=false;
    private float clickCooldown=0; 
    private bool startGameTrigger=false;
    
    //  Creem la variable del botó.
    public Button startButton;
    
    //  Creem les variables del text del temps i el número de temps,número d'intents.
    public TextMeshProUGUI timeText;
    private double timeNum;
    private int AttemptsNum;

    //  Creem les variables dels textos de la UI.
    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI title;
    public TextMeshProUGUI attempts;
    public TextMeshProUGUI score;

    //  Creem la variable per emmagatzemar el score actual.
    private int scoreNum;

    // Creem la llista de referencies dels audios
    public AudioClip[] audioClips;

    //  Creem la variable per l'audio d'origen.
    public AudioSource audioSource; 


    //  Constructor de 'Start'
    void Start()
    {

        // Fem la comprovació de que el botó d'inici estigui definit
        if (startButton != null)
        {
            // En el cas que estigui definit afegim un listener per executar la funció BotoAccio quan es faci clic al botó d'inici.
            startButton.onClick.AddListener(BotoAccio);
        }

        // Indiquem el text del títol del joc que volem que és mostri.
        title.text = "Sonic\n Memory";

        // Inicialitzem el nombre d'intents a 0
        AttemptsNum = 0;

        // Indiquem el nombre d'intents inicials amb el text que volem que és mostri.
        attempts.text = "     Attempts: " + AttemptsNum;

        // Indiquem la millor puntuació de la memòria i la mostrem, si no existeix cap
        // millor puntuació guardada, li indicarem que el valor per defecte sigui 0.
        bestTime.text = "Best Time: " + PlayerPrefs.GetInt("BestScore", 0);

        // Inicialitzem el text de temps a 0
        timeText.text = "Time: " + 0;

        // Inicialitzem la puntuació actual a 0
        scoreNum = 0;

        // Indiquem la puntuació inicial amb el text que volem que és mostri.
        score.text = "Score: " + scoreNum;

        // Buidem les cartes seleccionades per poder tornar a seleccionar cartes.
        cardsSelected[0] = null;
        cardsSelected[1] = null;

        // Reiniciem el comptador de cartes encertades a 0
        cartesAdivinades = 0;

        // Recuperem totes les cartes amb l'etiqueta "CardTag".
        cards = GameObject.FindGameObjectsWithTag("CardTag");

        // Recuperem els objectes amb l'etiqueta "CardTagSon".
        cardSons = GameObject.FindGameObjectsWithTag("CardTagSon");

    }

    //  Constructor de 'Update'
    void Update()
    {

        //  Actualizem el temps de la partida.
        if (startGameTrigger)
        {
            timeNum += Time.deltaTime;
            timeText.text = ("Time: " + (int)timeNum);

        }

        //  Indiquem el temps de retras per poder pulsar alguna tecla.
        if (clickTrigger)
        {
            clickCooldown += Time.deltaTime;
            if (clickCooldown >= 1)
            {
                clickCooldown = 0;
                clickTrigger = false;
            }
        }

        //  Fem la comprovació si hi ha dues cartes seleccionades
        if (cardsSelected[0] != null && cardsSelected[1] != null)
        {
            AnimatorStateInfo stateInfo0 = cardsSelected[0].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo stateInfo1 = cardsSelected[1].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

            //  Un cop comprovat, fem la animació i cridem a la funció de la comprovació de concidencies de 'Ids'.
            if (stateInfo0.IsName("CardUp") && stateInfo1.IsName("CardUp"))
            {
                ComprovacioIds();
            }
        }

        // En el cas de que s'hagi endevinat 8 parelles, indicarem que finalitzi el joc.
        if (cartesAdivinades == 8)
        {

            //  Indiquem que és reprodueixi el so de final de joc.
            audioSource.PlayOneShot(audioClips[4]);

            // Fem la comprovació de que si el temps és inferior al fet anteriorment mostrar
            // un missatge de felicitació.
            int bestScore = PlayerPrefs.GetInt("BestScore", int.MaxValue);
            if (timeNum < bestScore)
            {
                title.text = "¡New Sonic Best Score!";
                PlayerPrefs.SetInt("BestScore", (int)timeNum);
                title.color = Color.yellow;
            }

            // Desactivem el trigger de començar joc perque és finalitzi després d'uns segons.
            startGameTrigger = false;

            // Evitem que s'executi més d'una vegada
            cartesAdivinades += 1;

            // Canvia a la pantalla de final de partida, cridant a la funció de finalitzar escena.
            Invoke("FinalizarEscena", 5);

        }
        }
    
        //  Creem la funció per avisar al GameManager que alguna ha estat seleccionada.
        public void CartaSeleccionada(GameObject cartaSeleccionada){

        //  Indiquem que si hem seleccionat una carta, en el cas no hagi seleccionat cap carta anteriorment,
        //  reprodueixi el so cada vegada que seleccioni una des les dues cartes que ens permet el joc, per
        //  aixó mateix fem la comprovació de les cartes seleccionades estiguin en null per evitar més seleccions.
        if(cardsSelected[0]==null && cardsSelected[1]==null){
            cardsSelected[0]=cartaSeleccionada.gameObject;
            audioSource.PlayOneShot(audioClips[1]);
        }else{
            cardsSelected[1]=cartaSeleccionada.gameObject;
            audioSource.PlayOneShot(audioClips[1]);
        }   
    }

    //  Creem la funció per fer la comprovació de concidencies de 'Ids'.
    public void ComprovacioIds(){

        //  Indiquem una altre vegada que si hem seleccionat una carta, en el cas no hagi seleccionat cap carta anteriorment,
        //  però en aquest cas per fer la comprovació de les concidencies dels 'Ids'.
        if (cardsSelected[0]!=null && cardsSelected[1]!=null){

            //  Ara l'indiquem que si l'id de la primera carta seleccionada i la segona carta no concideixen les torni a girar/amagar.
            if(cardsSelected[0].GetComponent<Card>().getId()!=cardsSelected[1].GetComponent<Card>().getId()){
                cardsSelected[0].GetComponent<Card>().amagar();
                cardsSelected[1].GetComponent<Card>().amagar();

                //  Sumem un intent més.
                AttemptsNum += 1;
                attempts.text = "     Attempts: " + AttemptsNum;

                //  Reproduïm el so de parella incorrecta.
                audioSource.PlayOneShot(audioClips[2]);

                //  I fem la des-selecció de les cartes.
                borrarSeleccionats(12);
            }else{
                //  En el cas de que les cartes concideixen incrementem 'cartesAdivinades'.
                cartesAdivinades++;

                // Afegim 10 punts al score actual cada vegada que trobem una concidencia de 'Ids', osigui de cartes. 
                scoreNum += 10; 
                score.text = "Score: " + scoreNum;

                //  Reproduïm el so de parella correcta.
                audioSource.PlayOneShot(audioClips[3]);

                //  Fem desapareixer les cartes en cas que concideixen.
                Destroy(cardsSelected[0]); 
                Destroy(cardsSelected[1]);

                //  I fem la des-selecció de les cartes.
                borrarSeleccionats(12);
            }
        }
    }

    //  Creem la funció de la selecció de cartes.
    public bool SeleccioCartes(){

        //  Retornem la selecció de les cartes buides.
        return cardsSelected[0]==null || cardsSelected[1]==null;
    }

    //  Creem la funció per fer des-selecció de les cartes una vegada feta la comprovació.
    public void borrarSeleccionats(int num){
        
        //  Indiquem que buidi les seleccions de les cartes, per poder tornar a seleccionar cartes.
        if(num==0){
            cardsSelected[0]=null;
        }
        if(num==1){
            cardsSelected[1]=null;
        }
        if(num==12){
            cardsSelected[0]=null;
            cardsSelected[1]=null;
        }
    }

    //  Creem la funció per fer la barrejar les cartes de manera aleatoria.
    void Barrejar(GameObject[] llistaRandom)
    {
        //  Creem una variable de tipus enter per emmagatzemar la llista de objectes, la qual obtindra les cartes de forma aleatoria.
        int n = llistaRandom.Length;

        //  Fem un bucle 'for' per recorrer la llista i anar-la rellenant de forma aleatoria que és el que se preten, utilitzant 'Random'
        //  i anar asignant cada posició de forma aleatoria.
        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(i, n);
            GameObject temp = llistaRandom[i];
            llistaRandom[i] = llistaRandom[randomIndex];
            llistaRandom[randomIndex] = temp;
        }
    }

    //  Creem la funció per fer que el botó d'iniciar el joc.
    void BotoAccio()
    {
        //  Indiquem que és reprodueixi el so de final de joc.
        audioSource.PlayOneShot(audioClips[0]);
        cardSons[0].GetComponent<Card>().setStartVar(true);

        //  Cambiem l'estat del botó.
        startButton.gameObject.SetActive(false);
        startGameTrigger=true;
        
        //  Actualitzem l'estat de la 'UI'.
        title.text="";
        AttemptsNum=0;
        attempts.text= "     Attempts: " + AttemptsNum;

        //  Mitjançant un 'for each', asignem les 'Ids' i figures a les cartes.
        foreach(GameObject card in cardSons){
            card.GetComponent<Card>().setId(id_double);
            id_double=id_double+0.5;
            Material materialSource = Resources.Load<Material>("Materials/FigureMaterial" + card.GetComponent<Card>().getId());
            card.GetComponent<Card>().getFiguraRenderer().material = materialSource;
        }
        //  Cridem a la funció per barrejar les cartes.
        Barrejar(cards);

        //  Coloquem les cartes en el ordre obtingut per el 'Random'.
        int i=0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                //  Creem un Vector3 per colocar les cartes en ordre adequat apartir de la posició actual.
                Vector3 posicion = new Vector3((float)((x * 2)-2), (float)-1.3, (y * 2)-2);
                cards[i].transform.position = posicion; //  Posició asignada de cada carta.
                i++;
            }
        }
    }

    //  Creem la funció per fer que és finalitzi l'escena.
    void FinalizarEscena(){

        //  Creem la variable per la cual accedirem a l'escena.
        string currentSceneName = SceneManager.GetActiveScene().name;

        //  Carregem l'escena.
        SceneManager.LoadScene(currentSceneName);
    }

    //  Setters i getters per controlar les accións que és fan al fer els clics.
    public bool canClickTrigger(){
        return !clickTrigger;
    }

    public void setClickTrigger(bool clickTriger){
        clickTrigger = clickTriger;
    }
}
