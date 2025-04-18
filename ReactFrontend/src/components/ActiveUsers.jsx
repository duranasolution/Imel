import React from 'react';
import UserList from './Users';

const ActiveUsers = () => {
  return <UserList defaultRoleFilter="active" title="Aktivni korisnici" />;
};

export default ActiveUsers;
