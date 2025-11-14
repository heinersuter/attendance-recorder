import YearList from './year/YearList'
import logo from '/attendance-recorder.svg'
import './App.css'

function App() {

  return (
      <>
          <h1><img src={logo} alt="Attendance Recorder logo"/> Attendance Recorder</h1>

          <div className="d-flex gap-4 align-items-start">
              <div>
                  <YearList />
                  {/*<YearList SelectedYearChanged="ChangeYear"/>*/}
              </div>
              <div>
                  {/*<WeekList Year="_selectedYear" SelectedWeekChanged="ChangeWeek"/>*/}
              </div>
              <div>
                  {/*<DateList Year="_selectedYear" Week="_selectedWeek" SelectedDateChanged="ChangeDate"/>*/}
              </div>
              <div className="flex-grow-1">
                  {/*<WorkingDay Date="_selectedDate"></WorkingDay>*/}
              </div>
          </div>
      </>
  )
}

export default App
