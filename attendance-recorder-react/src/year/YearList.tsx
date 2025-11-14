import { useEffect, useState } from 'react';
import { ApiClient } from '../ApiClient.Generated';

function YearList() {
    const [years, setYears] = useState<number[] | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const apiClient = new ApiClient('http://localhost:11515');
        
        apiClient.getYears()
            .then((data) => {
                setYears(data);
            })
            .catch((err) => {
                setError(err.message || 'Failed to load years');
                console.error('Error loading years:', err);
            });
    }, []);

    if (error) {
        return (
            <>
                <h2>Years</h2>
                <p>Error: {error}</p>
            </>
        );
    }

    if (years === null) {
        return (
            <>
                <h2>Years</h2>
                <p>Loading years...</p>
            </>
        );
    }

    return (
        <>
            <h2>Years</h2>
            <ul>
                {years.map((year) => (
                    <li key={year}>{year}</li>
                ))}
            </ul>
        </>
    );
};

export default YearList;