import { useEffect, useState } from "react";
import { ApiClient } from "../ApiClient.Generated";

interface WeekListProps {
  year: number | null;
  onWeekSelected?: (week: number) => void;
}

function WeekList({ year, onWeekSelected }: WeekListProps) {
  const [weeks, setWeeks] = useState<number[] | null>(null);
  const [selectedWeek, setSelectedWeek] = useState<number | null>(null);

  useEffect(() => {
    if (year === null) {
      setWeeks(null);
      return;
    }

    const apiClient = new ApiClient("http://localhost:11515");

    apiClient
      .getWeeks(year)
      .then((data) => {
        setWeeks(data);
        if (data && data.length > 0) {
          const firstWeek = data[0];
          setSelectedWeek(firstWeek);
          onWeekSelected?.(firstWeek);        }
      })
      .catch((err) => {
        console.error("Error loading weeks:", err);
      });
  }, [year]);

  const handleClick = (week: number) => {
    setSelectedWeek(week);
    onWeekSelected?.(week);
  };

  return (
    <>
      <h2>Weeks</h2>
      {weeks === null ? (
        <p>Loading weeks...</p>
      ) : (
        <ul className="flex flex-col gap-2">
          {weeks.map((week) => (
            <li key={week}>
              <button
                className={`btn ${week === selectedWeek ? "btn-primary" : "btn-outline"}`}
                onClick={() => handleClick(week)}
              >
                {week}
              </button>
            </li>
          ))}
        </ul>
      )}
    </>
  );
}

export default WeekList;
