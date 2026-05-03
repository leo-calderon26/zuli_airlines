<template>
  <nav class="relative bg-primary after:pointer-events-none after:absolute after:inset-x-0 after:bottom-0 after:h-px after:bg-white/10">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div class="relative flex h-20 items-center justify-between">
        <div class="flex items-center justify-center sm:items-stretch sm:justify-start">
          <div class="flex shrink-0 items-center">
            <router-link to="/admin/"><img src="../assets/logoZuli.svg" alt="Your Company" class="h-20 w-auto" /></router-link>
          </div>
        </div>
        <div class="flex items-center justify-center sm:items-stretch sm:justify-start">
          
          <h1 class="font-bold text-heading md:text-5xl lg:text-3xl"><span class="text-transparent bg-clip-text bg-white">Panel Administrativo</span></h1>

        </div>
        <div class="absolute inset-y-0 right-0 flex items-center sm:static sm:inset-auto">
          <div class="flex shrink-0 items-center">
            <router-link to="/admin/profile-settings"><img src="../assets/person.svg" alt="Your Company" class="h-12 w-auto" /></router-link>
          </div>
        </div>
      </div>
    </div>
  </nav>
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