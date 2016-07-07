using System;
using System.Collections.Generic;

using SystemLogic;
using SystemLogic.Interfaces;

using NUnit.Framework;

namespace SysLogicTests
{
    [TestFixture]
    public class SCfgHelperSystemLogicTest
    {
        private ISCfgCommand SC { get; set; }

        [Test]
        public void VerifyCorrectCreateServiceArgumentsListTest()
        {
            // arrange
            ConfigOperation operationType = ConfigOperation.Create;

            SC = new SCfgCommand(operationType);
            SC.ServiceName = "CODIS Application Server";

            SC.Arguments.Add("DisplayName", "CASWinServer");
            SC.Arguments.Add("binPath", @"C:\SourceCode\DMSS\CODIS\Trunk\Source\Product\Debug\CODISApplicationServer.exe");
            SC.Arguments.Add("obj", @"CODIS\GDISSQLServer");
            SC.Arguments.Add("password", "C0dis@ecs");

            // act
            string argumentsList = ((SCfgCommand)SC).BuildArgumentsString(operationType);

            // assert
            Assert.That(argumentsList, Is.EqualTo("create \"CODIS Application Server\" DisplayName=\"CASWinServer\" binPath=\"C:\\SourceCode\\DMSS\\CODIS\\Trunk\\Source\\Product\\Debug\\CODISApplicationServer.exe\" obj=\"CODIS\\GDISSQLServer\" password=\"C0dis@ecs\""));
        }

        [Test]
        public void VerifyCorrectDeleteServiceArgumentsListTest()
        {
            // arrange
            ConfigOperation operationType = ConfigOperation.Delete;

            SC = new SCfgCommand(operationType);
            SC.ServiceName = "CODIS Application Server";            

            // act
            string argumentsList = ((SCfgCommand)SC).BuildArgumentsString(operationType);

            // assert
            Assert.That(argumentsList, Is.EqualTo("delete \"CODIS Application Server\""));
        }
    }
}
