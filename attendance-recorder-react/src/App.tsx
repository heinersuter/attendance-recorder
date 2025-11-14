import { useState } from "react";
import YearList from "./year/YearList";
import WeekList from "./week/WeekList";
import logo from "/attendance-recorder.svg";
import "./App.css";

function App() {
  const [selectedYear, setSelectedYear] = useState<number | null>(null);

  const handleYearSelected = (year: number) => {
    setSelectedYear(year);
  };

  return (
    <>
      <h1 className="flex items-center gap-3">
        <img src={logo} alt="Attendance Recorder logo" className="w-8 h-8" />
        Attendance Recorder
      </h1>

      <div className="flex gap-4 items-start">
        <div>
          <YearList onYearSelected={handleYearSelected} />
        </div>
        <div>
          <WeekList year={selectedYear} />
        </div>
        <div>
          {/*<DateList Year="_selectedYear" Week="_selectedWeek" SelectedDateChanged="ChangeDate"/>*/}
        </div>
        <div className="flex-grow-1">
          {/*<WorkingDay Date="_selectedDate"></WorkingDay>*/}
        </div>
      </div>
    </>
  );
}

export default App;
