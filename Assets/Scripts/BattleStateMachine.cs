using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleStateMachine : MonoBehaviour
{

    /* How to Include stuff
     * Basically you want to increase Curr Speed* to everyone every second all is in Waiting
     * When a monster reaches 100%speed, set that monsters statemachine in Selecting/Action
     * In Action Set speed back to 0, and repeat once all monsters are back in Waiting
     * 100% -> attack

       5.
       5.10.15.20.25.30.35
       60.61.62.63.64.65.66
     * 
     */


    public enum PerformAction
    {
        INCREMENT,
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }
    public PerformAction battleState;
    public List<TurnHandler> PerformList = new List<TurnHandler>();
    public List<GameObject> AllMonstersOnBattlefield = new List<GameObject>();
    public List<GameObject> FriendlyMonstersOnBattlefield = new List<GameObject>();
    public List<GameObject> EnemyMonstersOnBattlefield = new List<GameObject>();

    //public buttons for abilities
    public Button ability1;
    public Button ability2;
    public Button ability3;
    public Button ability4;


    public Text dmgText;
    public int tempCount =0;

    [Header("Instantiating Monsters")]
    [SerializeField] public GameObject Hero1;
    [SerializeField] public GameObject Hero2;
    [SerializeField] public GameObject Hero3;

    [SerializeField] public List<GameObject> FriendlyPositions = new List<GameObject>();
    [SerializeField] public List<GameObject> EnemyPositions = new List<GameObject>();
    [SerializeField] public List<GameObject> monsterPrefab = new List<GameObject>();


    private GameObject monsterInTurn;




    void Start()
    {
        AddMonstersToBattlefield();
        //Populate Lists of Monsters
        AllMonstersOnBattlefield.AddRange(GameObject.FindGameObjectsWithTag("Monster"));
        foreach (GameObject obj in AllMonstersOnBattlefield)
        {
            if (obj.GetComponent<BaseMonster>().wild)
                EnemyMonstersOnBattlefield.Add(obj);
            else
                FriendlyMonstersOnBattlefield.Add(obj);
        }

        //Update lists if someone dies
        //search for all monsters on the battlefield
        //add them in a list, then use this list - Remove from list if Dead(?)

        battleState = PerformAction.INCREMENT;
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            case (PerformAction.INCREMENT):
                IncrementAllMonsters();
                //only increment if all monsters are in state : WAITING OR DEAD
                break;
            case (PerformAction.WAIT):
                if (PerformList.Count > 0)
                    battleState = PerformAction.TAKEACTION;
                //Stay here forever
                break;
            case (PerformAction.TAKEACTION):
                // Take Action -> Perform action
                /* Getting the MonsterStateMachine of the attacker
                 * Sending in the information of whom it is attacking, and also changing the state to ACTION
                 */
                tempCount = PerformList.Count;
                PerformList[0].AttackerObject.GetComponent<MonsterStateMachine>().MonsterToAttack(PerformList[0].SelectedGameObject);
                PerformList[0].AttackerObject.GetComponent<MonsterStateMachine>().currentState = MonsterStateMachine.TurnState.ACTION;
                battleState = PerformAction.PERFORMACTION;
                //TEMP ACTION MEETERS
                temporaryPrintStatement();
                break;
            case (PerformAction.PERFORMACTION):
                /* Added a temp variable to see whenever PerformList is reduced in size
                 * Just a simple >1 would give us bugs if there was multiple monsters reaching 100 speed
                 */
                if (tempCount > PerformList.Count)
                    battleState = PerformAction.INCREMENT;
                //BUG FIX IF MULTIPLE AT FULL SPEED
                //if (PerformList.Count > 1)
                //    battleState = PerformAction.TAKEACTION;


                // set back to increment
                //set health/stamina etc of all monsters involved, set that monsters
                //state back -> Increment

                break;
        }
    }





    public void CollectTurns(TurnHandler input)
    {
        PerformList.Add(input);
    }

    

   
    //TEMP PRINT FUNCITON
    // Want this in a standalone script
    private void temporaryPrintStatement()
    {
        PerformList[0].SelectedGameObject.GetComponent<BaseMonster>().currHP -= PerformList[0].Ability.GetComponent<BaseAbilities>().damage + PerformList[0].AttackerObject.GetComponent<BaseMonster>().currStrength;
        print(PerformList[0].AttackerObject.GetComponent<BaseMonster>().name + " attacks "+ PerformList[0].SelectedGameObject.GetComponent<BaseMonster>().name + " With " + PerformList[0].Ability.GetComponent<BaseAbilities>().name + " for " + PerformList[0].Ability.GetComponent<BaseAbilities>().damage + " damage");
        if (PerformList[0].SelectedGameObject.GetComponent<BaseMonster>().currHP < 1)
            PerformList[0].SelectedGameObject.GetComponent<MonsterStateMachine>().currentState = MonsterStateMachine.TurnState.DEAD;
    }



    void IncrementAllMonsters()
    {
        /*
         * Making sure all Monsters in the battlefield are in Waiting before Incrementing
         */

        foreach (GameObject obj in AllMonstersOnBattlefield)
        {
            // If any monster is in ANY OTHER STATE, THAN DEAD OR WAITING
            if (obj.GetComponent<MonsterStateMachine>().currentState != MonsterStateMachine.TurnState.WAITING
                && obj.GetComponent<MonsterStateMachine>().currentState != MonsterStateMachine.TurnState.DEAD)
            {
                print("Debug1");
                battleState = PerformAction.WAIT;
                monsterInTurn = obj;
                return;
            }
        }


        /*
         * Looping through all monsters
         * Increasing their Current speed by 1 * their speed multiplier
         *  - So if a monster has 1 speed multiplier, their speed will be increased by 1. 1,2,3,4,5
         *  - But if a monster has 2 speed multiplier, their speed will be increased by 2. 1,3,5,7 etc
         * 
         * Room for improvement, it is biased if multiple monsters reach 100>\
         * We need to change this, because it is biased based on order in  the list
         * 
         * Currently showing abilities if monster that has reached 100 is Not Wild
         */
        foreach (GameObject obj in AllMonstersOnBattlefield)
        {
            if (obj.GetComponent<MonsterStateMachine>().currentState != MonsterStateMachine.TurnState.DEAD)
                obj.GetComponent<BaseMonster>().currSpeed += 1 * obj.GetComponent<BaseMonster>().speedMultiplier;
            // NOT A GOOD WAY< CHANGE LATER
            if (obj.GetComponent<BaseMonster>().currSpeed >= 100)
            {
                obj.GetComponent<MonsterStateMachine>().currentState = MonsterStateMachine.TurnState.SELECTING;
                if (obj.GetComponent<BaseMonster>().wild == false)
                    ShowAbilityButtons(obj);
            }
        }
    }


    public void ShowAbilityButtons(GameObject monster)
    {
        ability1.GetComponentInChildren<Text>().text = monster.GetComponent<BaseMonster>().abilities[0].GetComponent<BaseAbilities>().name + "\nEnergy:[" + (int)monster.GetComponent<BaseMonster>().abilities[0].GetComponent<BaseAbilities>().energyGain + "/" + (int)monster.GetComponent<BaseMonster>().currEnergy + "]";
        ability2.GetComponentInChildren<Text>().text = monster.GetComponent<BaseMonster>().abilities[1].GetComponent<BaseAbilities>().name + "\nEnergy:[" + (int)monster.GetComponent<BaseMonster>().abilities[1].GetComponent<BaseAbilities>().energyGain + "/" + (int)monster.GetComponent<BaseMonster>().currEnergy + "]";
        ability3.GetComponentInChildren<Text>().text = monster.GetComponent<BaseMonster>().abilities[2].GetComponent<BaseAbilities>().name + "\nEnergy:[" + (int)monster.GetComponent<BaseMonster>().abilities[2].GetComponent<BaseAbilities>().energyGain + "/" + (int)monster.GetComponent<BaseMonster>().currEnergy + "]";
        ability4.GetComponentInChildren<Text>().text = monster.GetComponent<BaseMonster>().abilities[3].GetComponent<BaseAbilities>().name + "\nEnergy:[" + (int)monster.GetComponent<BaseMonster>().abilities[3].GetComponent<BaseAbilities>().energyGain + "/" + (int)monster.GetComponent<BaseMonster>().currEnergy + "]";
    }


    public void AddMonstersToBattlefield()
    {
        Hero1.transform.position = FriendlyPositions[0].transform.position;
        Hero2.transform.position = FriendlyPositions[1].transform.position;
        Hero3.transform.position = FriendlyPositions[2].transform.position;
        Hero1.GetComponent<MonsterStateMachine>().startPosition = FriendlyPositions[0].transform.position;
        Hero2.GetComponent<MonsterStateMachine>().startPosition = FriendlyPositions[1].transform.position;
        Hero3.GetComponent<MonsterStateMachine>().startPosition = FriendlyPositions[2].transform.position;

        int howManyMonsters = Random.Range(0, EnemyPositions.Count) + 1;

        for (int x = 0; x< howManyMonsters; x++)
        {
            int whichMonsterPrefab = Random.Range(0, monsterPrefab.Count);
            GameObject temporary = Instantiate(monsterPrefab[whichMonsterPrefab], EnemyPositions[x].transform.position, EnemyPositions[x].transform.rotation);
            temporary.GetComponent<BaseMonster>().wild = true;
        }
        //GameObject enemy1 = Instantiate(enemyPrefab, EnemyPosition1.transform.position, EnemyPosition1.transform.rotation);
        //GameObject enemy2 = Instantiate(enemyPrefab, EnemyPosition2.transform.position, EnemyPosition1.transform.rotation);
        //GameObject enemy3 = Instantiate(enemyPrefab, EnemyPosition3.transform.position, EnemyPosition1.transform.rotation);

    }


    public void AbilityPressed(int button)
    {
        if (monsterInTurn != null && !(monsterInTurn.GetComponent<BaseMonster>().wild) && battleState == PerformAction.WAIT)
        {
            if (monsterInTurn.GetComponent<BaseMonster>().currEnergy + monsterInTurn.GetComponent<BaseMonster>().abilities[button].GetComponent<BaseAbilities>().energyGain >= 0)
            {
                monsterInTurn.GetComponent<MonsterStateMachine>().PlayerSelectMove(button);
                monsterInTurn = null;
            }
        }
    }


    //End of Code
}

