import { useEffect, useState } from "react";
import { useApiClient } from "../ApiClientContext";

interface DayListProps {
  year: number | null;
  week: number | null;
  onDaySelected?: (day: Date) => void;
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat("de-CH").format(date);
}

function DayList({ year, week, onDaySelected }: DayListProps) {
  const [days, setDays] = useState<Date[] | null>(null);
  const [selectedDay, setSelectedDay] = useState<Date | null>(null);
  const apiClient = useApiClient();

  useEffect(() => {
    if (year === null || week === null) {
      setDays(null);
      return;
    }

    apiClient
      .getDays(year, week)
      .then((data) => {
        setDays(data);
        if (data && data.length > 0) {
          const firstDay = data[0];
          setSelectedDay(firstDay);
          onDaySelected?.(firstDay);
        }
      })
      .catch((err) => {
        console.error("Error loading days:", err);
      });
  }, [year, week]);

  const handleClick = (day: Date) => {
    setSelectedDay(day);
    onDaySelected?.(day);
  };

  return (
    <>
      <h2>Days</h2>
      {days === null ? (
        <p>Loading days...</p>
      ) : (
        <ul className="flex flex-col gap-2">
          {days.map((day) => (
            <li key={formatDate(day)}>
              <button
                className={`btn w-full ${day === selectedDay ? "btn-primary" : "btn-outline"}`}
                onClick={() => handleClick(day)}
              >
                {formatDate(day)}
              </button>
            </li>
          ))}
        </ul>
      )}
    </>
  );
}

export default DayList;
