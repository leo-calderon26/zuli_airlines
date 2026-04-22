<template>
    <header class="top-nav-bar">
        <div class="content">
            <router-link to="/"><img src="../assets/logoZuli.svg" alt="Logo" class="logo"></router-link>
            <div class="top-content">
                Panel Administrativo
            </div>
            <router-link to="/administrativo" class="admin-link">
                <img src="../assets/person.svg" alt="Admin Icon" class="admin-icon">
                <span>Admin</span>
            </router-link>
        </div>
    </header>
</template>

<script>
    export default {
        
    }
</script>

<style>
.top-nav-bar {
    background-color: #711717;
    width: 100%;
}

.content {
  width: 100%;
  display: flex;
  justify-content: flex-start;
  align-items: center;
}

.top-content {
    width: 100%;
    align-items: center;
    gap: 20px;
    font-weight: 600;
    color: white;
    font-size: 26px;
}

.logo {
  width: auto;
  height: clamp(48px, 8vw, 100px);
  max-width: 100%;
}

.admin-link {
  margin-left: auto;
  margin-right: 20px;
  color: white;
  text-decoration: none;
  font-weight: 600;
  display: flex;
  flex-direction: column;
  align-items: center;
}

nav ul {
  display: flex; 
  align-items: center;
  list-style: none;
  margin: 0;
  padding: 0;
}

nav ul li {
  padding: 0 18px;
}

nav ul li + li {
  border-left: 1px solid rgba(255, 255, 255, 0.5);
}

.nav-links {
  color: white;
  text-decoration: none;
  font-weight: 600;
}

.nav-links a {
  color: white;
  text-decoration: none;
  font-weight: 600;
  padding: 10px 14px;
  display: inline-block;
}

.nav-links a.router-link-exact-active {
  background-color: rgba(0, 0, 0, 0.25);
  border-radius: 6px;
}

</style>