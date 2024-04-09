let provider;
let signer;
var connected = "false";


async function connectWallet () {
	// Check if Phantom wallet is installed
	if (window.solana && window.solana.isPhantom) {
	    // Phantom wallet is installed
	    const solana = window.solana;

	    // Connect to the Phantom wallet
	    solana.connect().then(() => {
	        
	        // You can now use solana object to interact with the wallet
	        // For example, you can get the user's public key
	        const publicKey = solana.publicKey.toString();
	        connected = "true";
	        unityGame.SendMessage('ConnectWalletButton', 'ReceiveAddress', publicKey);
	    }).catch((error) => {
	        console.error('Failed to connect to Phantom wallet:', error);
	    });
	} else {
	    // Phantom wallet is not installed, handle this case accordingly
	    console.error('Phantom wallet is not installed');
	    alert('Phantom wallet is not installed');
	}
}

function checkConnected() {
  unityGame.SendMessage('Managers', 'successConnected', connected);
}