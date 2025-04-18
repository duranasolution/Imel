import React from 'react';
import UserList from './Users';

const InactiveUsers = () => {
  return <UserList defaultRoleFilter="inactive" title="Neaktivni korisnici" />;
};

export default InactiveUsers;
