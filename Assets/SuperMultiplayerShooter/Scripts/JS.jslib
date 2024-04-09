mergeInto(LibraryManager.library, {
  connectWalletJS: function () {
    connectWallet();
  },

  checkConnectedJS: function () {
    checkConnected();
  },

  payArcadeJS: function () {
    payArcade();
  },

  getArcadeBalanceJS: function () {
    getArcadeBalance();
  }
})