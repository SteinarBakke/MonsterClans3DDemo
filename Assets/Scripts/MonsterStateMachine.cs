using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterStateMachine : MonoBehaviour
{
    private BattleStateMachine battleState;
    public BaseMonster monster;


    /*
    * Basically, you want this class to control and decide the different user interactions, possibly also "Auto"?
    */

    public enum TurnState
    {
        PROCESSING,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    public TurnState currentState;

    [Header("Progress bars")]
    public Image HealthBarImage;
    public Text HealthBarText;
    public Text NameText;
    public Text LvlText;

    private float stamina = 0f;

    //WILL NOT USE THIS LATER
    //WILL USE BATTLE COORDINATES
    public Vector3 startPosition;

    //ienumerator actions (FOR ANIMATIONS)
    private bool actionStarted = false;
    private bool actionComplete = false;
    private float animSpeed = 12f;

    //CHANGE THIS LATER
    private GameObject selectedEnemy = null;





    void Start()
    {
        currentState = TurnState.WAITING;
        battleState = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
    }

    void Update()
    {
        UpdateProgressBars();
        /*
         * Switch between the different TurnStates
         */
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                /* State for after Selection, waiting for battleManager
                 * BattleManager will put the Monster in Action
                 */ 
                break;
            case (TurnState.WAITING):
                /* Idle State
                 * 
                 */ 
                break;
            case (TurnState.SELECTING):
                //if AI do AISelectMove
                //Change this, NOT TAG LATER
                if (monster.wild)
                    AISelectMove();
                //else
                //    PlayerSelectMove();
                print("here");
                currentState = TurnState.PROCESSING;
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                //currentState = TurnState.WAITING;
                //set speed back to desired start position
                //could be 0 (complete reset), or depending on ability
                //increase Energy based on the ability
                //change currEnergy
                //change currStamina
                break;
            case (TurnState.DEAD):
                StartCoroutine(TimeToDie());
                break;
            default:
                break;
        }
    }

    public void PlayerSelectMove(int ability)
    {

        //SEPERATE IF AI OR PLAYER
        /* TEMP RANDOM
         */
        TurnHandler myAttack = new TurnHandler();
        myAttack.AttackerObject = this.gameObject;

        monster.currEnergy += monster.abilities[ability].GetComponent<BaseAbilities>().energyGain;

        int selectedChamp = Random.Range(0, battleState.EnemyMonstersOnBattlefield.Count);
        float tempHp = battleState.EnemyMonstersOnBattlefield[selectedChamp].GetComponent<BaseMonster>().currHP > 0 ? battleState.EnemyMonstersOnBattlefield[selectedChamp].GetComponent<BaseMonster>().currHP : 10000;
        for (int x = 0; x < battleState.EnemyMonstersOnBattlefield.Count; x++)
        {
            if (battleState.EnemyMonstersOnBattlefield[x].GetComponent<BaseMonster>().currHP <= tempHp && battleState.EnemyMonstersOnBattlefield[x].GetComponent<MonsterStateMachine>().currentState != TurnState.DEAD)
            {
                selectedChamp = x;
                tempHp = battleState.EnemyMonstersOnBattlefield[x].GetComponent<BaseMonster>().currHP;
            }
        }

        myAttack.SelectedGameObject = battleState.EnemyMonstersOnBattlefield[selectedChamp];
        myAttack.Ability = monster.abilities[ability]; //0 or 1 FOR NOW! SHOULD HOLD ID OF ABILITY
        battleState.CollectTurns(myAttack);
    }

    void AISelectMove()
    {
        /*
    * SUPER SIMPLE FOR NOW
    * BUT IMPROVE THIS FOR SUPER COOL AI MECHANICS
     */
        //Will return 0 or 1
        int ability;
        ability = Random.Range(0, 2); // (only first 2 abilities)
        if (monster.currEnergy >= 100) // THIS IS BAD. LOOK AT ABILITY COST, RATHER THAN HARDCODING!!!!!!!
            ability = 3;
        else if ((monster.currHP < (monster.baseHP / 3)) && (monster.currEnergy >= 60))
            ability = 2;

        monster.currEnergy += monster.abilities[ability].GetComponent<BaseAbilities>().energyGain;
        TurnHandler myAttack = new TurnHandler();
        myAttack.AttackerObject = this.gameObject;
        // Select random monster
        // THIS SHOULD NOT BE RANDOM, IT'S SILLY
        int selectedChamp = Random.Range(0, battleState.FriendlyMonstersOnBattlefield.Count);
        float tempHp = battleState.FriendlyMonstersOnBattlefield[selectedChamp].GetComponent<BaseMonster>().currHP > 0? battleState.FriendlyMonstersOnBattlefield[selectedChamp].GetComponent<BaseMonster>().currHP : 10000;
        for (int x = 0; x < battleState.FriendlyMonstersOnBattlefield.Count; x++)
        {
            if (battleState.FriendlyMonstersOnBattlefield[x].GetComponent<BaseMonster>().currHP <= tempHp && battleState.FriendlyMonstersOnBattlefield[x].GetComponent<MonsterStateMachine>().currentState != TurnState.DEAD)
            {
                selectedChamp = x;
                tempHp = battleState.FriendlyMonstersOnBattlefield[x].GetComponent<BaseMonster>().currHP;
            }
        }
        //selectedEnemy = battleState.HeroesInBattle[selectedChamp];
        myAttack.SelectedGameObject = battleState.FriendlyMonstersOnBattlefield[selectedChamp];
        myAttack.Ability = monster.abilities[ability]; //0 or 1 FOR NOW! SHOULD HOLD ID OF ABILITY
        battleState.CollectTurns(myAttack);

    }

    void UpdateProgressBars()
    {
        //TEMP FOR SHOW
        //monster.currStamina -= Time.deltaTime *5;
        //scale healthImage
        //Enemy Monster UI
        HealthBarImage.fillAmount = monster.currHP / monster.baseHP;
        HealthBarText.text = (int)monster.currHP + "/" + (int)monster.baseHP;
        LvlText.text = "Lvl " + (int)monster.currLVL;
        NameText.text = monster.name;
    }

    public void MonsterToAttack(GameObject target)
    {
        selectedEnemy = target;
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted) { yield break; }

        actionStarted = true;
        // animate the enemy near the target to attack
        // selectedEnemy is currently NULL
        Vector3 targetPosition;
        if (monster.wild)
            targetPosition = new Vector3(selectedEnemy.transform.position.x + 2.5f, selectedEnemy.transform.position.y, selectedEnemy.transform.position.z);
        else
            targetPosition = new Vector3(selectedEnemy.transform.position.x - 2.5f, selectedEnemy.transform.position.y, selectedEnemy.transform.position.z);

        //Moving towards Enemey
        while (MoveTowardsEnemy(targetPosition)){ yield return null; }

        // wait for damage
        yield return new WaitForSeconds(0.5f);
        //ANIMATE DAMAGE ABILITY


        // animate back to start
        while (MoveTowardsStart(startPosition)) { yield return null; }
        // Set variable in Performlist to be "complete"
        battleState.PerformList.RemoveAt(0);
        // (keep performList for battleLog OR Add to new list called battlelog)

        actionStarted = false;
        //reset speed back to base speed
        monster.currSpeed = monster.baseSpeed;
        currentState = TurnState.WAITING;

        //battleState.battleState = BattleStateMachine.PerformAction.INCREMENT;

    }

    private IEnumerator TimeToDie()
    {
        // fly away, good, beautiful soul
        Vector3 target = new Vector3(0, 0, 90);
        while (RotateAroundSelf(target)){ yield return null; }
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool RotateAroundSelf(Vector3 target)
    {
        return target != (transform.forward = Vector3.RotateTowards(transform.forward, target, animSpeed * Time.deltaTime, 0.0f));
    }

}
