import { useEffect, useState } from "react";
import { useApiClient } from "../ApiClientContext";

interface YearListProps {
  onYearSelected?: (year: number) => void;
}

function YearList({ onYearSelected }: YearListProps) {
  const [years, setYears] = useState<number[] | null>(null);
  const [selectedYear, setSelectedYear] = useState<number | null>(null);
  const apiClient = useApiClient();

  useEffect(() => {
    apiClient
      .getYears()
      .then((data) => {
        setYears(data);
        if (data && data.length > 0 && selectedYear === null) {
          const firstYear = data[0];
          setSelectedYear(firstYear);
          onYearSelected?.(firstYear);
        }
      })
      .catch((err) => {
        console.error("Error loading years:", err);
      });
    // eslint-disable-next-line react-hooks/exhaustive-deps
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
                className={`btn w-full ${year === selectedYear ? "btn-primary" : "btn-outline"}`}
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
