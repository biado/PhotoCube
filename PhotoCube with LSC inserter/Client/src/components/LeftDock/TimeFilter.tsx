import React, { useState } from "react";
import { Filter } from "../Filter";
import { createFilter } from "../Middle/BottomDock/TagsetFilter";
import '../../css/LeftDock/TimeFilter.css';

/**
 * Component for adding a range query for Time tags.
 * Both the startTime and endTime must be filled for the 'Add filter' button to actually add the range filter.
 * To search for a distinct time, the same value for both startTime and endTime must be given.
 */
export const TimeFilter = (props: {
    activeFilters: Filter[],
    onFiltersChanged: (filter: Filter) => void
    onFilterReplacedByType: (oldFilter:Filter, newFilter: Filter) => void,
    onFilterRemovedByType: (filterType: string) => void
}) => {
    const initialValues = {
        previousStartTime: "",
        previousEndTime: "",
        startTime: "",
        endTime: ""
    };

    const [values, setValues] = useState(initialValues);
    const [buttonsDisabled, disableButtons] = useState(true);

    //Ref: https://dev.to/deboragaleano/how-to-handle-multiple-inputs-in-react-55el
    const handleInputChange = (e: { target: { name: any; value: any; }; }) => {
        disableButtons(false);
        const { name, value } = e.target;
        setValues({
            ...values,
            [name]: value,
        });
    }

    const addFilter = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        e.preventDefault();
        if (values.startTime !== "" && values.endTime !== "") {
            const filter: Filter = createFilter(values.startTime + "-" + values.endTime, 0, "time");
            props.onFiltersChanged(filter);
            values.previousStartTime = values.startTime;
            values.previousEndTime = values.endTime;
            disableButtons(true);
        }
    }

    const replaceFilter = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        e.preventDefault();
            if (values.startTime !== values.previousStartTime || values.endTime !== values.previousEndTime) {
                const oldFilter: Filter = createFilter(values.previousStartTime + "-" + values.previousEndTime, 0, "time");
                const newFilter: Filter = createFilter(values.startTime + "-" + values.endTime, 0, "time");
                if (props.activeFilters.some(af => af.type === "time")) {
                    props.onFilterReplacedByType(oldFilter, newFilter);
                    disableButtons(true);
                }
            }
    }

    const onClear = () => {
        if (values.startTime !== "" || values.endTime !== "") {
            setValues(initialValues);
            props.onFilterRemovedByType("time");
            disableButtons(true);
        }
    }

    return (
        <div className="time filter">
            <form>
                <div className="start time field">
                    <p>Start:</p>
                    <div className="input container">
                        <input className="input field" type="text" placeholder="00:00"
                        value={values.startTime}
                        onChange={(e) => handleInputChange(e)}
                        name="startTime">
                        </input>
                    </div>
                </div>
                <div className="end time field">
                    <p>End:</p>
                    <div className="input container">
                        <input className="input field" type="text" placeholder="23:59"
                        value={values.endTime}
                        onChange={(e) => handleInputChange(e)}
                        name="endTime">
                        </input>
                    </div>
                </div>
            </form>
            <div id="date-filter-buttons">
                <button disabled={buttonsDisabled} onClick={() => onClear()}>Clear</button>
                <button disabled={buttonsDisabled} onClick={(e) => (values.previousStartTime === "" && values.previousEndTime === "") ? addFilter(e) : replaceFilter(e)}>Add filter</button>
            </div>
        </div>
    )
}