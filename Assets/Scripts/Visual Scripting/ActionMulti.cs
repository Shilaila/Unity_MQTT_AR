using System;
using System.Collections.Generic;
using Static;
using Unity.VisualScripting;

namespace Visual_Scripting
{
    /// <summary>
    /// Implements the "Interacting", "Combining" and "Custom" Action functionality of the TrainAR Framework,
    /// therefore it checks if the  user triggered the "Interact", "Combine" Buttons or triggered a CustomAction through
    /// scripts and allows to check this against the stored state in the visual statemachine.
    /// 
    /// This is the multi version of the action node. Therefore all specified actions in this node have to be completed before advancing.
    /// </summary>
    [UnitTitle("TrainAR: Action (Multi)")] //The title of the Unity
    [UnitSubtitle(
        "Waits for the user to perform all specified Actions\n(Interaction, Combination, or CustomAction) and triggers\nthe correct stateflow afterwards or incorrect stateflow\noutput if something else is performed")] //The unit group this belongs to 
    [UnitCategory("TrainAR")] //The folder it is stored at in the flow graph unit menu
    [TypeIcon(typeof(SelectOnEnum))]
    public class ActionMulti : Unit
    {
        /// <summary>
        /// The type of interaction to check against.
        /// </summary>
        public enum TrainARActionChoices
        {
            Interaction,
            Combination,
            CustomAction
        }
        
        /// <summary>
        /// What type of actions are accepted by this node.
        /// </summary>
        /// <value>Set in node in the editor. Default is Interaction.</value>
        [UnitHeaderInspectable("Actions: ")]
        public TrainARActionChoices actionChoice = TrainARActionChoices.Interaction;

        /// <summary>
        /// How many actions are in this node.
        /// </summary>
        /// <value>Set in node in the editor.Default is 1.</value>
        [UnitHeaderInspectable("Action count: ")]
        public int actionCount = 1;

        /// <summary>
        /// Store which actions were already triggered to decide when all are successfully triggered.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        private Dictionary<int, bool> actionStorage = new Dictionary<int, bool>();

        /// <summary>
        /// The Input port of the Unit that triggers the internal logic.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public ControlInput InputFlow { get; private set; }

        /// <summary>
        /// The Output port of the Unity that is triggered when the users interaction was CORRECT.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public ControlOutput AllActionsCorrect { get; private set; }

        /// <summary>
        /// The Output port of the Unity that is triggered when the users interaction was INCORRECT.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public ControlOutput IncorrectAction { get; private set; }

        /// <summary>
        /// The Name of the first correct interactable for this step.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public List<ValueInput> ARCombinableName1s { get; private set; } = new List<ValueInput>();

        /// <summary>
        /// The Name of the second correct interactable for this step.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public List<ValueInput> ARCombinableName2s { get; private set; } = new List<ValueInput>();

        /// <summary>
        /// The Graphreference stores the current position in the flow graph to revisit it on Event/Action triggers.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public GraphReference graphReference { get; private set; }

        /// <summary>
        /// The stored name of the interactables to check them against the interaction.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        private List<string> combinableNames1 = new List<string>();
        /// <summary>
        /// The stored name of the interactables to check them against the interaction.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        private List<string> combinableNames2 = new List<string>();

        /// <summary>
        /// The Name of the correct interactable for this step.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public List<ValueInput> ARInteractableNames { get; private set; } = new List<ValueInput>();

        /// <summary>
        /// The stored name of the interactables to check them against the interaction.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        private List<string> interactableNames = new List<string>();

        /// <summary>
        /// The correct parameter for the custom event.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        [DoNotSerialize]
        public List<ValueInput> CorrectParameterTexts { get; private set; } = new List<ValueInput>();

        /// <summary>
        /// The correct parameter handed by the custom event.
        /// </summary>
        /// <value>Set in node in the editor.</value>
        private List<string> customActionParameters = new List<string>();

        /// <summary>
        /// Defines the Nodes input, output and value ports.
        /// </summary>
        protected override void Definition()
        {
            //Override the actionCount if not between 1 and 20
            if (actionCount is < 1 or > 20) actionCount = 1;
            
            //Defining the Input port of the flow & Node Logic
            InputFlow = ControlInput("", NodeLogic);

            //Static Output flow that is triggered dependend on the NodeLogic
            AllActionsCorrect = ControlOutput("All correct");
            IncorrectAction = ControlOutput("Incorrect");

            //Define ValueInputs and ValueOutputs based on the selected action to perform
            switch (actionChoice)
            {
                case TrainARActionChoices.Interaction:
                    for (int i = 0; i < actionCount; i++)
                    {
                        ARInteractableNames.Add(ValueInput<string>("Correct object [" + (i + 1) + "]", string.Empty));
                    }
                    break;
                case TrainARActionChoices.Combination:
                    for (int i = 0; i < actionCount; i++)
                    {
                        ARCombinableName1s.Add(
                            ValueInput<string>("Correct grabbed obj [" + (i + 1) + "]", string.Empty));
                        ARCombinableName2s.Add(ValueInput<string>("Correct stationary obj [" + (i + 1) + "]",
                            string.Empty));
                    }
                    break;
                case TrainARActionChoices.CustomAction:
                    for (int i = 0; i < actionCount; i++)
                    {
                        CorrectParameterTexts.Add(ValueInput<string>("Correct parameter [" + (i + 1) + "]",
                            string.Empty));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The NodeLogic that is triggered when an input flow is detected on the controlInput.
        ///
        /// This sets the instruction text at the UI element at the top of the smartphone and registers an Action
        /// with StateInformation that can be triggered the TrainAR StateChecker.
        /// </summary>
        /// <param name="flow">The current flow of the graph</param>
        /// <returns>Null, as the flow is stopped until the registered event is triggered.</returns>
        private ControlOutput NodeLogic(Flow flow)
        {
            //Make the action storage exactly the size of the count of correct actions
            actionStorage = new Dictionary<int, bool>(actionCount);
            for (int i = 0; i < actionCount; i++)
            {
                //Add default values to it
                actionStorage.Add(i, false);
            }
            
            switch (actionChoice)
            {
                case TrainARActionChoices.Interaction:
                    foreach (var interactableName in ARInteractableNames)
                    {
                        interactableNames.Add(flow.GetValue<string>(interactableName));
                    }

                    break;
                case TrainARActionChoices.Combination:
                    foreach (var combinableName in ARCombinableName1s)
                    {
                        combinableNames1.Add(flow.GetValue<string>(combinableName));
                    }
                    foreach (var combinableName in ARCombinableName2s)
                    {
                        combinableNames2.Add(flow.GetValue<string>(combinableName));
                    }
                    break;
                case TrainARActionChoices.CustomAction:
                    foreach (var parameter in CorrectParameterTexts)
                    {
                        customActionParameters.Add(flow.GetValue<string>(parameter));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Store the current position in the graph as we have to revisit it later
            graphReference = flow.stack.AsReference();

            //Register an Action and pass it to the TrainAR StateChecker to trigger it when ready
            Func<StateInformation, bool> triggerEvent = ContinueFlow;
            StatemachineConnector.RegisterNewStateChangeTrigger(triggerEvent);

            //Return null instead of ControLOutput to pause the graph flow until it is revisited by ContinueFlow
            return null;
        }

        /// <summary>
        /// Continues the flow when the corresponding action registered in the NodeLogic is triggered.
        /// </summary>
        /// <param name="stateInformation">The Information of the requested Statechange</param>
        private bool ContinueFlow(StateInformation stateInformation)
        {
            switch (actionChoice)
            {
                case TrainARActionChoices.Interaction:
                    //Checks if the user tries the correct action
                    if (stateInformation.interactionType == InteractionType.Interact)
                    {
                        //Checks if the correct interaction is in the list of possible interactions
                        if (interactableNames.Contains(stateInformation.primaryObjectName))
                        {
                            //Find out which one it was
                            for (int i = 0; i < interactableNames.Count; i++)
                            {
                                if (interactableNames[i] != stateInformation.primaryObjectName) continue;

                                //Check if this action, while correct, was already triggered, as it would be incorrect now then
                                if (actionStorage[i] == true)
                                {
                                    Flow.New(graphReference).Invoke(IncorrectAction);
                                    return false;
                                }
                                else
                                {
                                    //If this was not triggered before, store it as triggered now
                                    actionStorage[i] = true;
                                    
                                    //If there is no "false" (not triggered) action in the storage anymore, trigger the control flow for correct
                                    //This means the node is done and all needed Actions were triggered
                                    if (!actionStorage.ContainsValue(false))
                                    {
                                        Flow.New(graphReference).Invoke(AllActionsCorrect);
                                    }
                                    
                                    //Return this as a correct action regardless of the storage for user feedback while
                                    //staying in the node to check for the rest of Actions still not triggered
                                    return true;
                                }
                            }

                            //should not happen
                            Flow.New(graphReference).Invoke(IncorrectAction);
                            return false;
                        }
                        else
                        {
                            Flow.New(graphReference).Invoke(IncorrectAction); //Wrong object
                            return false;
                        }
                    }
                    else
                    {
                        Flow.New(graphReference).Invoke(IncorrectAction); //wrong action
                        return false;
                    }
                case TrainARActionChoices.Combination:
                    //Checks if the user tries the correct action
                    if (stateInformation.interactionType == InteractionType.Combine)
                    {
                        //Checks if the combinable names are even in the arrays
                        if ((combinableNames1.Contains(stateInformation.primaryObjectName) &&
                             combinableNames2.Contains(stateInformation.secondaryObjectName)))
                        {
                            //Check if those two combinables also belong together, if yes, trigger the control output
                            for (int i = 0; i < combinableNames1.Count; i++)
                            {
                                if (combinableNames1[i] != stateInformation.primaryObjectName ||
                                    combinableNames2[i] != stateInformation.secondaryObjectName) continue;
                                
                                //Check if this action, while correct, was already triggered, as it would be incorrect now then
                                if (actionStorage[i] == true)
                                {
                                    Flow.New(graphReference).Invoke(IncorrectAction);
                                    return false;
                                }
                                else
                                {
                                    //If this was not triggered before, store it as triggered now
                                    actionStorage[i] = true;
                                    
                                    //If there is no "false" (not triggered) action in the storage anymore, trigger the control flow for correct
                                    //This means the node is done and all needed Actions were triggered
                                    if (!actionStorage.ContainsValue(false))
                                    {
                                        Flow.New(graphReference).Invoke(AllActionsCorrect);
                                    }
                                    
                                    //Return this as a correct action regardless of the storage for user feedback while
                                    //staying in the node to check for the rest of Actions still not triggered
                                    return true;
                                }
                            }

                            //If the combinables are both in the list but dont belong together, trigger this
                            Flow.New(graphReference).Invoke(IncorrectAction);
                            return false;
                        }
                        else
                        {
                            Flow.New(graphReference).Invoke(IncorrectAction); //Wrong object
                            return false;
                        }
                    }
                    else
                    {
                        Flow.New(graphReference).Invoke(IncorrectAction); //wrong action
                        return false;
                    }
                case TrainARActionChoices.CustomAction:
                    //Checks if the user tries the correct action
                    if (stateInformation.interactionType == InteractionType.Custom)
                    {
                        //Checks if the custom action parameter is registered
                        if (customActionParameters.Contains(stateInformation.parameter))
                        {
                            //Find out which one it is triggered so we trigger the corresponding control output
                            for (int i = 0; i < customActionParameters.Count; i++)
                            {
                                if (customActionParameters[i] != stateInformation.parameter) continue;

                                //Check if this action, while correct, was already triggered, as it would be incorrect now then
                                if (actionStorage[i] == true)
                                {
                                    Flow.New(graphReference).Invoke(IncorrectAction);
                                    return false;
                                }
                                else
                                {
                                    //If this was not triggered before, store it as triggered now
                                    actionStorage[i] = true;
                                    
                                    //If there is no "false" (not triggered) action in the storage anymore, trigger the control flow for correct
                                    //This means the node is done and all needed Actions were triggered
                                    if (!actionStorage.ContainsValue(false))
                                    {
                                        Flow.New(graphReference).Invoke(AllActionsCorrect);
                                    }
                                    
                                    //Return this as a correct action regardless of the storage for user feedback while
                                    //staying in the node to check for the rest of Actions still not triggered
                                    return true;
                                }
                            }

                            //Should not happen
                            Flow.New(graphReference).Invoke(IncorrectAction);
                            return false;
                        }
                        else
                        {
                            Flow.New(graphReference).Invoke(IncorrectAction); //Wrong object
                            return false;
                        }
                    }
                    else
                    {
                        Flow.New(graphReference).Invoke(IncorrectAction); //wrong action
                        return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
