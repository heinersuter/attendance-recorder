import { useEffect, useState } from "react";
import type { IntervalDto, WorkingDayDto } from "../ApiClient.Generated";
import {
  getInvariantDateString,
  getWeekdayName,
} from "../common/date-functions";
import { useApiClient } from "../ApiClientContext";
import { IconClockMinus } from "@tabler/icons-react";

interface WorkingDayProps {
  day: Date | null;
}

function WorkingDay({ day }: WorkingDayProps) {
  const [workingDay, setWorkingDay] = useState<WorkingDayDto | null>(null);
  const apiClient = useApiClient();

  useEffect(() => {
    if (day === null) {
      setWorkingDay(null);
      return;
    }

    apiClient
      .getWorkingDay(getInvariantDateString(day))
      .then((data) => {
        setWorkingDay(data);
      })
      .catch((err) => {
        console.error("Error loading working day:", err);
      });
  }, [day]);

  const handleClick = (interval: IntervalDto) => {
    if (day === null) {
      setWorkingDay(null);
      return;
    }

    const mergePromise = interval.isActive
      ? // If an active interval is deleted, an inactive merge is added
        apiClient.postInactiveMerge(
          getInvariantDateString(day),
          interval.start,
          interval.end,
        )
      : // If an inactive interval is deleted, an active merge is added
        apiClient.postActiveMerge(
          getInvariantDateString(day),
          interval.start,
          interval.end,
        );

    mergePromise
      .then((data) => {
        setWorkingDay(data);
      })
      .catch((err) => {
        console.error("Error merging interval:", err);
      });
  };

  return (
    <>
      {day === null || workingDay === null ? (
        <p>Loading working day...</p>
      ) : (
        <>
          <h2>{getWeekdayName(day)}</h2>
          <table className="w-full">
            <tbody>
              {workingDay.intervals.map((interval) => (
                <tr
                  className={interval.isActive ? "" : "text-gray-500"}
                  key={interval.start}
                >
                  <td>
                    {interval.start} ... {interval.end}
                  </td>
                  <td>{interval.duration}</td>
                  <td className="text-center">
                    {interval.isActive ? "(active)" : "(inactive)"}
                  </td>
                  <td className="text-end">
                    <button
                      type="button"
                      className="btn p-0 h-auto"
                      onClick={() => handleClick(interval)}
                      title="Delete"
                    >
                      <IconClockMinus
                        size={20}
                        color={interval.isActive ? "white" : "#6b7280"}
                      />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}
    </>
  );
}

export default WorkingDay;
