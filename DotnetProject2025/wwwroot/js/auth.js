import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.5/firebase-app.js";
import { getAuth, signInWithPopup, GoogleAuthProvider, RecaptchaVerifier, signInWithPhoneNumber } from "https://www.gstatic.com/firebasejs/10.12.5/firebase-auth.js";

const firebaseConfig = {
    apiKey: "AIzaSyBfimuOHaepWQZgUKufVsDbbgOTkiJd2S8",
    authDomain: "dotnetproject2025.firebaseapp.com",
    projectId: "dotnetproject2025",
    storageBucket: "dotnetproject2025.appspot.com",
    messagingSenderId: "31152390676",
    appId: "1:31152390676:web:3b9d89940e0a1f7743dd01",
    measurementId: "G-GD9YB2RBXN"
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

window.loginWithGoogle = function () {
    const provider = new GoogleAuthProvider();
    signInWithPopup(auth, provider)
        .then((result) => {
            return result.user.getIdToken();
        })
        .then((idToken) => {
            return fetch('/credential/google-login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ token: idToken })
            });
        })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text) });
            }
            return response.json();
        })
        .then(data => {
            console.log('User logged in successfully');
            window.location.href = "/Home/Index";
        })
        .catch(error => {
            if (error.code === 'auth/popup-closed-by-user') {
                console.error('The popup was closed by the user before completing the sign in.');
            } else {
                console.error('Error during login: ', error);
            }
        });
};

window.loginWithPhoneNumber = function () {
    const phoneNumber = document.getElementById('phoneNumber');
    const appVerifier = new RecaptchaVerifier("sign-in-button", {
        "size": "invisible",
        "callback": function (response) {
            // reCAPTCHA solved, you can proceed with
            phoneAuthProvider.verifyPhoneNumber(phoneNumber).
            onSolvedRecaptcha();
        }
    }, auth);

    signInWithPhoneNumber(auth, phoneNumber, appVerifier)
        .then((confirmationResult) => {
            const code = prompt('Enter the OTP sent to your phone');
            return confirmationResult.confirm(code);
        })
        .then((result) => {
            return result.user.getIdToken();
        })
        .then((idToken) => {
            return fetch('/credential/phone-login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ token: idToken })
            });
        })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text) });
            }
            return response.json();
        })
        .then(data => {
            console.log('User logged in successfully');
        })
        .catch((error) => {
            if (error.code === 'auth/popup-closed-by-user') {
                console.error('The popup was closed by the user before completing the sign in.');
            } else {
                console.error('Error during phone login: ', error);
            }
        });
};
