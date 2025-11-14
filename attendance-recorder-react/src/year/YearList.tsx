import { useEffect, useState } from "react";
import { ApiClient } from "../ApiClient.Generated";

interface YearListProps {
  onYearSelected?: (year: number) => void;
}

function YearList({ onYearSelected }: YearListProps) {
  const [years, setYears] = useState<number[] | null>(null);
  const [selectedYear, setSelectedYear] = useState<number | null>(null);

  useEffect(() => {
    const apiClient = new ApiClient("http://localhost:11515");

    apiClient
      .getYears()
      .then((data) => {
        setYears(data);
        if (data && data.length > 0) {
          const firstYear = data[0];
          setSelectedYear(firstYear);
          onYearSelected?.(firstYear);
        }
      })
      .catch((err) => {
        console.error("Error loading years:", err);
      });
  }, []);

  if (years === null) {
    return (
      <>
        <h2>Years</h2>
        <p>Loading years...</p>
      </>
    );
  }

  const handleYearClick = (year: number) => {
    setSelectedYear(year);
    onYearSelected?.(year);
  };

  return (
    <>
      <h2>Years</h2>
      {years === null ? (
        <p>Loading years...</p>
      ) : (
        <ul className="flex flex-col gap-2">
          {years.map((year) => (
            <li key={year}>
              <button
                className={`btn ${year === selectedYear ? "btn-primary" : "btn-outline"}`}
                onClick={() => handleYearClick(year)}
              >
                {year}
              </button>
            </li>
          ))}
        </ul>
      )}
    </>
  );
}

export default YearList;
