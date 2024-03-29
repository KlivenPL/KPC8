﻿using Microsoft.Extensions.DependencyInjection;
using Player._Infrastructure.States;
using Player.GuiLogic.StateMachine.States;

namespace Player.GuiLogic.StateMachine {
    internal class GuiStateManager {
        private readonly IServiceProvider serviceProvider;

        // private GuiStateResolver guiStateResolver;

        public GuiStateManager(IServiceProvider serviceProvider /*GuiStateResolver guiStateResolver*/) {
            //this.guiStateResolver = guiStateResolver;
            this.serviceProvider = serviceProvider;
        }

        public IGuiState CurrentState { get; private set; }

        public void Initialize() {
            CurrentState = serviceProvider.GetRequiredService<StopGuiState>();
            CurrentState.OnEnter();
        }

        private void SetState<TGuiState>() where TGuiState : IGuiState {
            CurrentState.OnExit();
            CurrentState = serviceProvider.GetRequiredService<TGuiState>();
            CurrentState.OnEnter();
        }

        internal class GuiStateSetter {
            private readonly GuiStateManager guiStateManager;

            public GuiStateSetter(GuiStateManager guiStateManager) {
                this.guiStateManager = guiStateManager;
            }

            protected void SetState<TGuiState>() where TGuiState : IGuiState {
                guiStateManager.SetState<TGuiState>();
            }
        }
    }
}
