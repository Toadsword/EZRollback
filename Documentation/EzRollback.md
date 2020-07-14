#EZRollback Documentation 

##Introduction
This documentation will explain how to use the framework. Before being able to use the rollback in your game engine, the framework will need to know what information need to be stored to perform a rollback and what needs to be executed when simulating frames.

##Glossary

*Rollback* : Going back in time in the state of the game
*Simulate frames* : Going forward in time. Works like fixed update... But we do it a certain number of time at once.
*Fixed frame* : A frame recorded with the fixed time step of Unity (default : every 0.02Sec)


##Important scripts
*RollbackManager* : The main manager that deals with all the rollback mechanism. Make callbacks at the right time and manage the global status of the frames and the game.
*IRollbackBehaviour* : Abstract class, inheriting from Monobehaviour, that implements all the required functions from the rollbackmanager callbacks.
*IRollbackInputManager* : Complement abstract manager that stores player's inputs and rollback them. Is necessary to use rollback in your game for inputs(for networking for example). Have extra functions that allow input correction for players.
*RollbackElement<T>* : Data structures conveniently designed to store all the information the rollback system need about your variable.
*RollbackInputBaseActions*: Base data structure used to store the input data. Optimised to use the minimum required space for network transfere.

##Example scripts
For *IRollbackBehaviour* : PositionRollback and RotationRollback, that implements rollback for the position and the rotation of the linked object.
For *RollbackInputManager* : SampleRollbackInputManager, implementing the basic Unity input system to the needs of the rollback system. Can be found in Tests/Runtime/InputDelayComparer.

##Transitionning your scripts
Basically, you will need to implement what the rollback needs to do on your scripts. For every scripts that changes important values every frames, you will need to replace :
- *Monobehaviour* with *IRollbackBehaviour*
	- Implement the asked functions 
	- Put all the Update/Fixed update needs into the Simulate(). Simulate will be called every fixed update. Puting all the update information into Simulate works too, as the rollbackManager only registers in history every fixed time step.
	- Put your rollbackable variable into a defined struct. Create that struct with the RollbackElement<> wrapper.
	- Call the right functions at the right time with the overriden functions from IRollbackBehaviour
		- SetValueFromFrameNumber()
		- DeleteFrames()
		- SaveFrame()
- *Your input manager* with *IRollbackInputManager* :
	- Implement GetCurrentActionsValue(), where you store your current inputs from your input manager.
	- Correct all input uses in your game with the one from the new manager.