import React, { Component } from 'react';
import { Switch, Route, Link } from 'react-router-dom';
import { TimePeriodIndex } from './TimePeriodIndex';
import { TimePeriodCreate } from './TimePeriodCreate';
import { TimePeriodDetails } from './TimePeriodDetails';

export class TimePeriods extends Component {
    render() {
        return (
            <div>
                <h1 id="tabelLabel">
                    Time Periods
                    <span className="float-right">
                        <Switch>
                            <Route
                                path="/time-periods"
                                exact
                                render={() => (
                                    <Link to="/time-periods/create" className="btn btn-primary btn-sm">
                                        Create
                                    </Link>
                                )}
                            />
                            <Route
                                path="/time-periods"
                                render={() => (
                                    <Link to="/time-periods" className="btn btn-outline-secondary btn-sm">
                                        close
                                    </Link>
                                )}
                            />
                        </Switch>
                    </span>
                </h1>
                <Switch>
                    <Route path="/time-periods/create" component={TimePeriodCreate} />
                    <Route path="/time-periods/:timePeriodId" component={TimePeriodDetails} />
                    <Route path="/time-periods" component={TimePeriodIndex} />
                </Switch>
            </div>
        );
    }
}
