import { useEffect, useState } from "react";
import { useApiClient } from "../ApiClientContext";

interface WeekListProps {
  year: number | null;
  onWeekSelected?: (week: number) => void;
}

function WeekList({ year, onWeekSelected }: WeekListProps) {
  const [weeks, setWeeks] = useState<number[] | null>(null);
  const [selectedWeek, setSelectedWeek] = useState<number | null>(null);
  const apiClient = useApiClient();

  useEffect(() => {
    if (year === null) {
      setWeeks(null);
      return;
    }

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
                className={`btn w-full ${week === selectedWeek ? "btn-primary" : "btn-outline"}`}
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
