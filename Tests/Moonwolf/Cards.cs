﻿using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;


namespace SotmWorkshop.Moonwolf
{
    [TestFixture()]
    public class Cards : Base
    {

        [Test]
        public void BlindRage_NoHeroTargets()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;
            var bb1 = PlayCard("BladeBattalion");
            var bb2 = PlayCard("BladeBattalion");

            QuickHPStorage(baron.CharacterCard, moonwolf.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, mdp, bb1, bb2);
            DecisionSelectCards = new[] { mdp, bb1, bb2 };

            var card = PlayCard("BlindRage");
            AssertInTrash(card);
            QuickHPCheck(0, 0, 0, 0, -2, -2, -2);
        }

        [Test]
        public void BlindRage_OneHeroTargets()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;
            var bb1 = PlayCard("BladeBattalion");
            var bb2 = PlayCard("BladeBattalion");

            QuickHPStorage(baron.CharacterCard, moonwolf.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, mdp, bb1, bb2);
            DecisionSelectCards = new[] { bunker.CharacterCard, bb1, bb2, bb1, bb2 };

            var card = PlayCard("BlindRage");
            AssertInTrash(card);
            QuickHPCheck(0, 0, -2, 0, 0, -4, -4);
        }

        [Test]
        public void BloodSacrifice()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            var priest = PutInHand("MoonPriestess");

            QuickHPStorage(baron.CharacterCard, moonwolf.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DecisionYesNo = true;
            DecisionSelectCards = new[] { priest };

            var card = PlayCard("BloodSacrifice");
            AssertInTrash(card);
            AssertInPlayArea(moonwolf, priest);
            QuickHPCheck(0, -2, 0, 0);
        }

        [Test]
        [Ignore("TODO")]
        public void ChannelTheMoon()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
        }

        [Test]
        public void CycleOfLife_StartOfTurn()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(moonwolf, 20);

            GoToEndOfTurn(baron);

            var card = PutIntoPlay("CycleOfLife");
            AssertInPlayArea(moonwolf, card);

            QuickHPStorage(baron.CharacterCard, moonwolf.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);

            GoToStartOfTurn(moonwolf);

            QuickHPCheck(0, 1, 0, 0);
        }

        [Test]
        public void CycleOfLife_EnvironmentTargets()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "MobileDefensePlatform");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;

            GoToEndOfTurn(baron);

            var card = PutIntoPlay("CycleOfLife");
            AssertInPlayArea(moonwolf, card);

            var target = PlayCard("ShieldGenerator");

            QuickHPStorage(baron.CharacterCard, moonwolf.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, target, mdp);
            DealDamage(moonwolf, target, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, -3, 0);

            QuickHPStorage(baron.CharacterCard, moonwolf.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, target, mdp);
            DealDamage(moonwolf, mdp, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -1);
        }


        [Test]
        public void DrawOutOfTheBeast_NoTokens()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "MobileDefensePlatform");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;

            GoToEndOfTurn(baron);

            int handCount = GetNumberOfCardsInHand(moonwolf);
            int trashCount = GetNumberOfCardsInTrash(moonwolf);
            int playCount = GetNumberOfCardsInPlay(moonwolf);
            AssertTokenPoolCount(pullofthemoon, 0);

            var card = PlayCard("DrawOutTheBeast");
            AssertInTrash(card);

            AssertNumberOfCardsInHand(moonwolf, handCount);
            AssertNumberOfCardsInPlay(moonwolf, playCount);
            AssertNumberOfCardsInTrash(moonwolf, trashCount + 1);
            AssertTokenPoolCount(pullofthemoon, 0);
        }

        [Test]
        public void DrawOutOfTheBeast_Tokens(
            [Values(1, 3, 5)] int tokens
            )
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "MobileDefensePlatform");
            StartGame();

            var priest = PutOnDeck("MoonPriestess");

            var mdp = GetMobileDefensePlatform().Card;

            AddTokensToPool(pullofthemoon, tokens);

            GoToEndOfTurn(baron);

            int handCount = GetNumberOfCardsInHand(moonwolf);
            int trashCount = GetNumberOfCardsInTrash(moonwolf);
            int playCount = GetNumberOfCardsInPlay(moonwolf);
            AssertTokenPoolCount(pullofthemoon, tokens);

            DecisionSelectNumber = tokens;
            DecisionSelectCard = priest;
            var card = PlayCard("DrawOutTheBeast");
            AssertInTrash(card);
            AssertInPlayArea(moonwolf, priest);

            AssertNumberOfCardsInHand(moonwolf, handCount);
            AssertNumberOfCardsInPlay(moonwolf, playCount + 1);
            AssertNumberOfCardsInTrash(moonwolf, trashCount + 1 + tokens - 1);
            AssertNumberOfCardsInRevealed(moonwolf, 0);
            AssertTokenPoolCount(pullofthemoon, 0);
        }

        [Test]
        public void ForcedChange_ChooseHand()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "MobileDefensePlatform");
            StartGame();

            var cycle = GetCard("CycleOfLife");
            DecisionSelectCard = cycle;
            DecisionMoveCardDestination = new MoveCardDestination(moonwolf.HeroTurnTaker.Hand);
            QuickShuffleStorage(moonwolf);
            var card = PlayCard("ForcedChange");
            AssertInTrash(card);
            AssertInHand(cycle);
            QuickShuffleCheck(1);
        }

        [Test]
        public void ForcedChange_ChoosePlay()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "MobileDefensePlatform");
            StartGame();

            var cycle = GetCard("CycleOfLife");
            DecisionSelectCard = cycle;
            DecisionMoveCardDestination = new MoveCardDestination(moonwolf.HeroTurnTaker.PlayArea);
            QuickShuffleStorage(moonwolf);
            var card = PlayCard("ForcedChange");
            AssertInTrash(card);
            AssertInPlayArea(moonwolf, cycle);
            QuickShuffleCheck(1);
        }

        [Test]
        //[Ignore("TODO")]
        public void FrenziedStrikes_SameTurn()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;

            GoToPlayCardPhase(moonwolf);
            
            var strikes = PutIntoPlay("FrenziedStrikes");

            GoToUsePowerPhase(moonwolf);
            PrintSeparator("Start");

            DecisionSelectTarget = mdp;
            QuickHPStorage(moonwolf.CharacterCard, mdp);
            SelectAndUsePower(moonwolf, out bool skipped);
            QuickHPCheck(-1, -2);

            QuickHPStorage(moonwolf.CharacterCard, mdp);
            SelectAndUsePower(moonwolf, out skipped);
            QuickHPCheck(-2, -2);
            
            GoToStartOfTurn(bunker);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        //[Ignore("TODO")]
        public void FrenziedStrikes_PlayedOutOfTurn()
        {
            SetupGameController("BaronBlade", "SotmWorkshop.Moonwolf", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;

            var strikes = PutIntoPlay("FrenziedStrikes");
            AssertNumberOfStatusEffectsInPlay(0);

            GoToStartOfTurn(moonwolf);

            AssertNumberOfStatusEffectsInPlay(1);
            GoToPlayCardPhase(moonwolf);
            GoToUsePowerPhase(moonwolf);

            DecisionSelectTarget = mdp;
            QuickHPStorage(moonwolf.CharacterCard, mdp);
            SelectAndUsePower(moonwolf, out bool skipped);
            QuickHPCheck(-1, -2);

            QuickHPStorage(moonwolf.CharacterCard, mdp);
            SelectAndUsePower(moonwolf, out skipped);
            QuickHPCheck(-2, -2);

            GoToStartOfTurn(bunker);
            AssertNumberOfStatusEffectsInPlay(0);
        }

    }
}
