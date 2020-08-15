using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Packages.EZRollback.Runtime.Scripts;
using Packages.EZRollback.Tests.Runtime.InputDelayComparer.Scripts;
using UnityEngine;
using UnityEngine.TestTools;

    public class CheckInputCorrection
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CheckInputCorrectionWithEnumeratorPasses()
        {
            //Setup RollbackManager with the input manager
            if (RollbackManager.Instance.GetComponent<SampleRollbackInputManager>() == null) {
                RollbackManager.Instance.gameObject.AddComponent<SampleRollbackInputManager>();
                RollbackManager.Instance.ResetRbInputManagerEvents();
            }
            
            RollbackManager.Instance.bufferSize = -1;
            RollbackManager.Instance.bufferRestriction = false;
            RollbackManager.Instance.registerFrames = true;
            RollbackManager.Instance.ClearRollbackManager();
            
            Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == 0);
            Assert.True(RollbackManager.Instance.GetMaxFramesNum() == 0);

            int playerId = RollbackManager.rbInputManager.AddPlayer();

            yield return new WaitForSeconds(0.1f);

            RollbackManager.Instance.Simulate(10);
            Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == 10);
            yield return new WaitForSeconds(0.1f);
            
            //Correct input of a certain frame number
            RollbackElementRollbackInputBaseActions playerInputHistory = RollbackManager.rbInputManager.GetPlayerInputHistory(playerId);
            
            RollbackInputBaseActions rbInput = new RollbackInputBaseActions(5);
            rbInput.SetHorizontalAxis(1.0f);
            rbInput.SetVerticalAxis(1.0f);
            rbInput.SetBit(3);

            playerInputHistory.CorrectValue(rbInput, 5);
            
            //Resimulate frames
            yield return new WaitForSeconds(0.1f);
            RollbackManager.Instance.ReSimulate(10);
            yield return new WaitForSeconds(0.1f);

            //Get corrected input
            RollbackInputBaseActions rbCorrectedInput = RollbackManager.rbInputManager.GetPlayerInputHistory(playerId).GetValue(5);
            Debug.Log(rbCorrectedInput.ToString());
            
            Assert.True(rbCorrectedInput.Equals(rbInput));
        }
    }
