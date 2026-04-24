/* eslint-disable @typescript-eslint/no-unused-vars */
import { useTracingDataStore } from '@quidgest/clientapp/stores'

import netAPI from '@quidgest/clientapp/network'
import qApi from '@/api/genio/quidgestFunctions.js'
import qProjArrays from '@/api/genio/projectArrays.js'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

/*
 * ====================================================
 * projectFuntions.js v1.0.0
 * http://www.quidgest.com
 * ====================================================
 * Copyright 2026 Quidgest, S.A.
 *
 * All project functions will be placed here.
 * ====================================================
 */

//*************** User functions ***************
function Age(birthdate)
{
	/// <summary>
	/// Calculate age
	/// </summary>
	/// <param name="birthdate">Date of birth to calculate the age</param>
/* eslint-disable indent */
//BEGIN_FUNCTION:b234e155-064a-482d-b629-f51ffe924c38
    const today = new Date();
    let age = 0;

    if (birthdate) {
        const birth = new Date(birthdate);

        age = today.getFullYear() - birth.getFullYear();

        if (
            birth.getMonth() > today.getMonth() ||(birth.getMonth() === today.getMonth() && birth.getDate() > today.getDate())
        ) {
            age--;
        }
    }

    return age;

//END_FUNCTION
// eslint-disable-next-line
/* eslint-enable indent */
}
function Average()
{
	/// <summary>
	/// Calculate the average price of properties
	/// </summary>
	return netAPI.executeServerFunction('Average', []);
}
function getCityTax(codProperty)
{
	/// <summary>
	/// Get the last added tax
	/// </summary>
	/// <param name="codProperty"></param>
	return netAPI.executeServerFunction('getCityTax', [codProperty]);
}
function averagePriceAgent(codagent)
{
	/// <summary>
	/// Calculates average price of properties sold by one agent
	/// </summary>
	/// <param name="codagent"></param>
	return netAPI.executeServerFunction('averagePriceAgent', [codagent]);
}
function propsNotBooking(visitDate)
{
	/// <summary>
	/// Return properties not booked in specific day
	/// </summary>
	/// <param name="visitDate"></param>
	return netAPI.executeServerFunction('propsNotBooking', [visitDate]);
}
function propsWithoutContacts()
{
	/// <summary>
	/// Only shows properties without contacts
	/// </summary>
	return netAPI.executeServerFunction('propsWithoutContacts', []);
}
function agentSameCountryPrope(codCity)
{
	/// <summary>
	/// List the agents that lives in same Country as Property
	/// </summary>
	/// <param name="codCity"></param>
	return netAPI.executeServerFunction('agentSameCountryPrope', [codCity]);
}

export default {
	Age,
	Average,
	getCityTax,
	averagePriceAgent,
	propsNotBooking,
	propsWithoutContacts,
	agentSameCountryPrope,
}
