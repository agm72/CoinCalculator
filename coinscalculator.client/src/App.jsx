import { useEffect, useState } from 'react';
import axios from "axios";
import './App.css';

function App() {
    const [targetAmount, setTargetAmount] = useState("");
    const [coinDenominations, setCoinDenominations] = useState("");
    const [result, setResult] = useState([]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        const denominations = coinDenominations
            .split(",")
            .map((d) => parseFloat(d.trim()));

        try {
            const response = await axios.post("/api/coin/calculate-coins", {
                targetAmount: parseFloat(targetAmount),
                coinDenominations: denominations,
            });
            setResult(response.data);
        } catch (error) {
            alert(error.response.data);
        }
    };

    return (
        <div style={{ padding: "20px" }}>
            <h1>Coin Calculator</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Target Amount:</label>
                    <input
                        type="number"
                        value={targetAmount}
                        onChange={(e) => setTargetAmount(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Coin Denominations (comma-separated):</label>
                    <input
                        type="text"
                        value={coinDenominations}
                        onChange={(e) => setCoinDenominations(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Calculate</button>
            </form>

            {result.length > 0 && (
                <div>
                    <h2>Result</h2>
                    <p>{result.join(", ")}</p>
                </div>
            )}
        </div>
    );
}

export default App;