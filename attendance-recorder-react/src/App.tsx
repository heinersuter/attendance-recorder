import { useState } from "react";
import YearList from "./selection/YearList";
import WeekList from "./selection/WeekList";
import DayList from "./selection/DayList";
import { ApiClient } from "./ApiClient.Generated";
import { ApiClientProvider } from "./ApiClientContext";
import logo from "/attendance-recorder.svg";
import "./App.css";

function App() {
  const [selectedYear, setSelectedYear] = useState<number | null>(null);
  const [selectedWeek, setSelectedWeek] = useState<number | null>(null);
  const [selectedDay, setSelectedDay] = useState<Date | null>(null);
  const apiClient = new ApiClient("http://localhost:11515");

  const handleYearSelected = (year: number) => {
    setSelectedYear(year);
  };
  const handleWeekSelected = (week: number) => {
    setSelectedWeek(week);
  };
  const handleDaySelected = (day: Date) => {
    setSelectedDay(day);
  };

  return (
    <ApiClientProvider apiClient={apiClient}>
      <h1 className="flex items-center gap-3">
        <img src={logo} alt="Attendance Recorder logo" className="w-8 h-8" />
        Attendance Recorder
      </h1>

      <div className="flex gap-4 items-start">
        <div className="w-32">
          <YearList onYearSelected={handleYearSelected} />
        </div>
        <div className="w-32">
          <WeekList year={selectedYear} onWeekSelected={handleWeekSelected} />
        </div>
        <div className="w-32">
          <DayList year={selectedYear} week={selectedWeek} onDaySelected={handleDaySelected} />
        </div>
        <div className="flex-grow-1">
          {/*<WorkingDay Date="_selectedDate"></WorkingDay>*/}
        </div>
      </div>
    </ApiClientProvider>
  );
}

export default App;
